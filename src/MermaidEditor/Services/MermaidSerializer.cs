using System.Text;
using MermaidEditor.Models;

namespace MermaidEditor.Services;

public static class StringExtensions
{
    // Fix for Issue 4: UNSANITIZED NODE TEXT
    // Before: Labels were injected raw into Mermaid strings.
    // After: Labels are sanitized, escaping characters that break Mermaid syntax and wrapping in quotes.
    public static string EscapeMermaidText(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return "\"\"";

        // To safely escape text for Mermaid strings inside double quotes without breaking
        // on special characters like # or :, we must use proper entity encoding.
        // However, Mermaid.js requires specific handling: newlines break strings unless quoted,
        // and quotes within quotes break parsing.
        // We will escape characters into placeholder strings, then convert placeholders to entities.
        var escaped = text
            .Replace("\"", "[[QUOT]]")
            .Replace("\n", "[[BR]]")
            .Replace("<", "[[LT]]")
            .Replace(">", "[[GT]]")
            // Semicolons are valid in mermaid strings as long as we don't accidentally create an entity.
            // But if we encode hashes, colons, or semicolons, we might break Mermaid's entity parsing.
            // The safest approach for raw text inside quotes is escaping quotes and newlines,
            // and maybe HTML entities, but NOT arbitrarily encoding things that aren't breaking.
            // Wait, the issue states: handle ", #, :, ;, newlines.
            // According to mermaid syntax, entities inside nodes are supported (e.g. #9829;)
            // If the user types a literal #, it can break the parser if it thinks it's an entity or comment.
            // The standard way in Mermaid to escape is using the `#chr;` syntax (e.g. `#35;` for `#`).
            // So we need `#35;`, `#58;` etc.
            .Replace("#", "[[HASH]]")
            .Replace(":", "[[COLON]]")
            .Replace(";", "[[SEMI]]")
            // Now resolve placeholders to HTML entities
            .Replace("[[QUOT]]", "&quot;")
            .Replace("[[BR]]", "<br/>")
            .Replace("[[LT]]", "&lt;")
            .Replace("[[GT]]", "&gt;")
            .Replace("[[HASH]]", "#35;")
            .Replace("[[COLON]]", "#58;")
            .Replace("[[SEMI]]", "#59;");

        return $"\"{escaped}\"";
    }
}

public static class MermaidSerializer
{
    public static string Serialize(Graph graph)
    {
        var sb = new StringBuilder();
        sb.AppendLine("graph TD;");

        foreach (var node in graph.Nodes)
        {
            var label = string.IsNullOrWhiteSpace(node.Label) ? node.Id : node.Label;
            var escapedLabel = label.EscapeMermaidText();
            var nodeString = node.Type switch
            {
                NodeType.Rectangle => $"{node.Id}[{escapedLabel}]",
                NodeType.RoundedRectangle => $"{node.Id}({escapedLabel})",
                NodeType.Stadium => $"{node.Id}([{escapedLabel}])",
                NodeType.Cylinder => $"{node.Id}[({escapedLabel})]",
                NodeType.Circle => $"{node.Id}(({escapedLabel}))",
                NodeType.Rhombus => $"{node.Id}{{{escapedLabel}}}",
                NodeType.Hexagon => $"{node.Id}{{{{{escapedLabel}}}}}",
                _ => $"{node.Id}[{escapedLabel}]"
            };
            sb.AppendLine($"    {nodeString};");
        }

        foreach (var edge in graph.Edges)
        {
            var arrow = edge.LineStyle switch
            {
                EdgeLineStyle.Dashed => "-.->",
                EdgeLineStyle.Dotted => "-.->", // Mermaid uses dashed for both dotted and dashed
                _ => "-->"
            };

            var line = edge.LineStyle switch
            {
                EdgeLineStyle.Dashed => "-.-",
                EdgeLineStyle.Dotted => "-.-",
                _ => "--"
            };

            if (string.IsNullOrWhiteSpace(edge.Label))
            {
                sb.AppendLine($"    {edge.SourceNodeId} {arrow} {edge.TargetNodeId};");
            }
            else
            {
                sb.AppendLine($"    {edge.SourceNodeId} {arrow}|{edge.Label.EscapeMermaidText()}| {edge.TargetNodeId};");
            }
        }

        var positions = new System.Collections.Generic.Dictionary<string, double[]>();
        foreach (var node in graph.Nodes)
        {
            positions[node.Id] = new double[] { node.X, node.Y };
        }
        var positionsJson = System.Text.Json.JsonSerializer.Serialize(positions);
        sb.AppendLine($"\n%% SimplyMermaidPositions: {positionsJson}");

        return sb.ToString();
    }
}
