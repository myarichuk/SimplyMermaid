using MermaidEditor.Models;
using MermaidEditor.Services;
using Xunit;

namespace MermaidEditor.Tests;

public class MermaidSerializerTests
{
    [Fact]
    public void Serialize_EmptyGraph_ReturnsOnlyHeader()
    {
        var graph = new Graph();
        var result = MermaidSerializer.Serialize(graph);
        Assert.Equal("graph TD;\n", result.Replace("\r\n", "\n"));
    }

    [Fact]
    public void Serialize_WithNodesAndEdges_ReturnsCorrectString()
    {
        var graph = new Graph();
        graph.Nodes.Add(new Node { Id = "A", Label = "Start" });
        graph.Nodes.Add(new Node { Id = "B", Label = "End" });
        graph.Edges.Add(new Edge { Id = "1", SourceNodeId = "A", TargetNodeId = "B", Label = "Go" });

        var result = MermaidSerializer.Serialize(graph);

        var expected = "graph TD;\n    A[Start];\n    B[End];\n    A -->|Go| B;\n";
        Assert.Equal(expected, result.Replace("\r\n", "\n"));
    }
}
