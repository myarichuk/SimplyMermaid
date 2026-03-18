using Microsoft.JSInterop;

namespace MermaidEditor.Services;

public class MermaidJsInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;

    public MermaidJsInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async ValueTask RenderMermaidAsync(string containerId, string mermaidCode)
    {
        await _jsRuntime.InvokeVoidAsync("mermaidInterop.renderMermaid", containerId, mermaidCode);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
