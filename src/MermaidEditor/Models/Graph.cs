namespace MermaidEditor.Models;

using System.Collections.Generic;

public class Graph
{
    public List<Node> Nodes { get; set; } = new();
    public List<Edge> Edges { get; set; } = new();
}
