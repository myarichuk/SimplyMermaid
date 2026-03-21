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
        Assert.Equal("graph TD;\n\n%% SimplyMermaidPositions: {}\n", result.Replace("\r\n", "\n"));
    }

    [Fact]
    public void Serialize_WithNodesAndEdges_ReturnsCorrectString()
    {
        var graph = new Graph();
        graph.Nodes.Add(new Node { Id = "A", Label = "Start", X = 0, Y = 0, Width = 120, Height = 60, IsSequence = false });
        graph.Nodes.Add(new Node { Id = "B", Label = "End", X = 0, Y = 0, Width = 120, Height = 60, IsSequence = false });
        graph.Edges.Add(new Edge { Id = "1", SourceNodeId = "A", TargetNodeId = "B", Label = "Go" });

        var result = MermaidSerializer.Serialize(graph);

        var expected = "graph TD;\n    A[\"Start\"];\n    B[\"End\"];\n    A -->|\"Go\"| B;\n\n%% SimplyMermaidPositions: {\"A\":[0,0,120,60,0],\"B\":[0,0,120,60,0]}\n";
        Assert.Equal(expected, result.Replace("\r\n", "\n"));
    }
}
