namespace MermaidEditor.Models;

using System.Collections.Generic;

public class Graph
{
    public DiagramType DiagramType { get; set; } = DiagramType.Flowchart;
    public List<Node> Nodes { get; set; } = new();
    public List<Edge> Edges { get; set; } = new();
}
