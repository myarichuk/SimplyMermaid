namespace MermaidEditor.Models;

public class Edge
{
    public string Id { get; set; } = string.Empty;
    public string SourceNodeId { get; set; } = string.Empty;
    public string TargetNodeId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public EdgeLineStyle LineStyle { get; set; } = EdgeLineStyle.Solid;
    public double Y { get; set; }
    public bool IsSequence { get; set; } = false;
}
