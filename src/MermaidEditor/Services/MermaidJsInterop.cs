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

    public async ValueTask FocusElementAsync(string elementId)
    {
        await _jsRuntime.InvokeVoidAsync("mermaidInterop.focusElement", elementId);
    }

    public async ValueTask CopyToClipboardAsync(string text)
    {
        await _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
