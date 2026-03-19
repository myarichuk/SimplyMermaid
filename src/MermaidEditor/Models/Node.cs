namespace MermaidEditor.Models;

public class Node
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; } = 120;
    public double Height { get; set; } = 60;
    public NodeType Type { get; set; } = NodeType.Rectangle;
}
