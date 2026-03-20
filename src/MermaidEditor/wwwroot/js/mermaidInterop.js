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
                    curve: 'basis',
                    htmlLabels: false
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
    },

    downloadText: function (filename, text) {
        const blob = new Blob([text], { type: 'text/plain' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = filename;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    },
    downloadSvgAsPng: function (svgId, filename) {
        const containerElement = document.getElementById(svgId);
        if (!containerElement) return;
        const svgElement = containerElement.querySelector('svg');
        if (!svgElement) return;

        let bbox = { x: 0, y: 0, width: 0, height: 0 };
        try { bbox = svgElement.getBBox(); } catch (e) {}

        const padding = 20;
        const width = bbox.width + padding * 2;
        const height = bbox.height + padding * 2;
        const minX = bbox.x - padding;
        const minY = bbox.y - padding;

        if (width <= padding * 2 || height <= padding * 2) return;

        const clonedSvg = svgElement.cloneNode(true);
        clonedSvg.setAttribute('width', width);
        clonedSvg.setAttribute('height', height);
        clonedSvg.setAttribute('viewBox', `${minX} ${minY} ${width} ${height}`);
        clonedSvg.setAttribute('xmlns', 'http://www.w3.org/2000/svg');

        const svgData = new XMLSerializer().serializeToString(clonedSvg);
        const svgBlob = new Blob([svgData], { type: 'image/svg+xml;charset=utf-8' });
        const url = URL.createObjectURL(svgBlob);

        const img = new Image();
        img.onload = function () {
            const canvas = document.createElement('canvas');
            canvas.width = width;
            canvas.height = height;
            const ctx = canvas.getContext('2d');
            ctx.drawImage(img, 0, 0);

            const pngUrl = canvas.toDataURL('image/png');
            const a = document.createElement('a');
            a.href = pngUrl;
            a.download = filename;
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
            URL.revokeObjectURL(url);
        };
        img.src = url;
    }
};
