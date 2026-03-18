window.mermaidInterop = {
    renderMermaid: async function (id, code) {
        try {
            mermaid.initialize({ startOnLoad: false, theme: 'default' });
            const element = document.getElementById(id);
            if (element) {
                // Clear any previous error states or content
                element.innerHTML = '';
                // Render SVG string
                const { svg } = await mermaid.render(`mermaid-${id}-svg`, code);
                element.innerHTML = svg;
            }
        } catch (error) {
            console.error("Mermaid parsing error:", error);
            const element = document.getElementById(id);
            if (element) {
                element.innerHTML = `<div style="color:red">Mermaid parsing error: ${error.message}</div>`;
            }
        }
    }
};