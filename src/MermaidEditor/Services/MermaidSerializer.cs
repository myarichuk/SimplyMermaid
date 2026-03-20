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
        return SerializeSelection(graph, graph.Nodes, graph.Edges);
    }

    public static string SerializeSelection(Graph graph, IEnumerable<Node> selectedNodes, IEnumerable<Edge> selectedEdges)
    {
        var sb = new StringBuilder();

        if (graph.DiagramType == DiagramType.Flowchart)
        {
            sb.AppendLine("graph TD;");

            foreach (var node in selectedNodes)
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

            foreach (var edge in selectedEdges)
            {
                var arrow = edge.LineStyle switch
                {
                    EdgeLineStyle.Dashed => "-.->",
                    EdgeLineStyle.Dotted => "-.->",
                    _ => "-->"
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
        }
        else if (graph.DiagramType == DiagramType.Sequence)
        {
            sb.AppendLine("sequenceDiagram");

            foreach (var node in selectedNodes.Where(n => n.Type == NodeType.Actor || n.Type == NodeType.Rectangle))
            {
                var label = string.IsNullOrWhiteSpace(node.Label) ? node.Id : node.Label;
                var escapedLabel = label.EscapeMermaidText();
                if (node.Type == NodeType.Actor)
                {
                    sb.AppendLine($"    actor {node.Id} as {escapedLabel}");
                }
                else
                {
                    sb.AppendLine($"    participant {node.Id} as {escapedLabel}");
                }
            }

            // Note: properly serializing Alt/Loop requires knowing what nodes are inside the fragment.
            // For a basic implementation based on the prompt, we output them as standard sequence blocks
            // or we just rely on the interactive visual canvas for drawing and don't deeply parse fragments yet,
            // but let's try a simple block output.
            var fragments = selectedNodes.Where(n => n.Type == NodeType.Fragment).ToList();

            // We sort edges by Y coordinate to get the correct chronological order
            var sortedEdges = selectedEdges.ToList();
            var edgePositions = new Dictionary<Edge, double>();
            foreach(var edge in sortedEdges)
            {
                var source = selectedNodes.FirstOrDefault(n => n.Id == edge.SourceNodeId);
                var target = selectedNodes.FirstOrDefault(n => n.Id == edge.TargetNodeId);
                if (source != null && target != null)
                {
                    edgePositions[edge] = (source.Y + target.Y) / 2.0;
                }
                else
                {
                    edgePositions[edge] = 0;
                }
            }
            sortedEdges = sortedEdges.OrderBy(e => edgePositions[e]).ToList();

            // To support fragments, we can see if an edge falls inside a fragment's bounding box
            int edgeIndex = 1;
            var activeFragments = new List<Node>();

            foreach (var edge in sortedEdges)
            {
                var arrow = edge.LineStyle switch
                {
                    EdgeLineStyle.Dashed => "-->>",
                    EdgeLineStyle.Dotted => "-)", // Represents async in sequence diagram
                    _ => "->>"
                };

                var displayLabel = string.IsNullOrWhiteSpace(edge.Label) ? $"({edgeIndex})" : edge.Label;
                var yPos = edgePositions[edge];

                // Check if we need to close any active fragments
                var fragmentsToClose = activeFragments.Where(f => yPos > (f.Y + f.Height/2)).ToList();
                foreach (var f in fragmentsToClose)
                {
                    sb.AppendLine("    end");
                    activeFragments.Remove(f);
                }

                // Check if we enter any new fragments
                var enteringFragments = fragments.Where(f =>
                    yPos >= (f.Y - f.Height/2) && yPos <= (f.Y + f.Height/2) && !activeFragments.Contains(f)).ToList();

                foreach (var f in enteringFragments)
                {
                    var fragLabel = string.IsNullOrWhiteSpace(f.Label) ? "alt" : f.Label;

                    // Simple logic: if label starts with loop or opt or alt, use that, else use rect
                    if (fragLabel.StartsWith("loop", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.AppendLine($"    loop {fragLabel.Substring(4).Trim()}");
                    }
                    else if (fragLabel.StartsWith("opt", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.AppendLine($"    opt {fragLabel.Substring(3).Trim()}");
                    }
                    else if (fragLabel.StartsWith("alt", StringComparison.OrdinalIgnoreCase))
                    {
                        sb.AppendLine($"    alt {fragLabel.Substring(3).Trim()}");
                    }
                    else
                    {
                        sb.AppendLine($"    rect rgb(200, 200, 200)");
                        sb.AppendLine($"    note right of {edge.SourceNodeId}: {fragLabel}");
                    }
                    activeFragments.Add(f);
                }

                sb.AppendLine($"    {edge.SourceNodeId}{arrow}{edge.TargetNodeId}: {displayLabel}");
                edgeIndex++;
            }

            // Close any remaining open fragments
            foreach (var f in activeFragments)
            {
                sb.AppendLine("    end");
            }
        }

        var positions = new System.Collections.Generic.Dictionary<string, double[]>();
        foreach (var node in selectedNodes)
        {
            positions[node.Id] = new double[] { node.X, node.Y };
        }
        var positionsJson = System.Text.Json.JsonSerializer.Serialize(positions);
        sb.AppendLine($"\n%% SimplyMermaidPositions: {positionsJson}");

        return sb.ToString();
    }
}
