window.mermaidInterop = {
    renderMermaid: async function (id, code) {
        try {
            mermaid.initialize({
                startOnLoad: false,
                theme: 'base',
                themeVariables: {
                    fontFamily: "'Caveat', cursive",
                    primaryColor: '#ffffff',
                    primaryTextColor: '#000000',
                    primaryBorderColor: '#000000',
                    lineColor: '#000000',
                    secondaryColor: '#f4f4f4',
                    tertiaryColor: '#e0e0e0'
                },
                flowchart: {
                    curve: 'basis'
                }
            });
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
    },
    focusElement: function (id) {
        const element = document.getElementById(id);
        if (element) {
            element.focus();
            element.select();
        }
    }
};