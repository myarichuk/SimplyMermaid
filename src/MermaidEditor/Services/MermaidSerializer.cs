using System.Text;
using MermaidEditor.Models;

namespace MermaidEditor.Services;

public static class MermaidSerializer
{
    public static string Serialize(Graph graph)
    {
        var sb = new StringBuilder();
        sb.AppendLine("graph TD;");

        foreach (var node in graph.Nodes)
        {
            var label = string.IsNullOrWhiteSpace(node.Label) ? node.Id : node.Label;
            sb.AppendLine($"    {node.Id}[{label}];");
        }

        foreach (var edge in graph.Edges)
        {
            if (string.IsNullOrWhiteSpace(edge.Label))
            {
                sb.AppendLine($"    {edge.SourceNodeId} --> {edge.TargetNodeId};");
            }
            else
            {
                sb.AppendLine($"    {edge.SourceNodeId} -- {edge.Label} --> {edge.TargetNodeId};");
            }
        }

        return sb.ToString();
    }
}
