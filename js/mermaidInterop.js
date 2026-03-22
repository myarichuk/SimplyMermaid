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
    getDimensions: function (id) {
        const element = document.getElementById(id);
        if (element) {
            return { width: element.clientWidth, height: element.clientHeight };
        }
        return { width: window.innerWidth, height: window.innerHeight };
    },
    downloadSvgAsPng: function (svgId, filename) {
        const containerElement = document.getElementById(svgId);
        if (!containerElement) return;

        let svgElement = containerElement.tagName.toLowerCase() === 'svg' ? containerElement : containerElement.querySelector('svg');
        if (!svgElement) return;

        let bbox = { x: 0, y: 0, width: 0, height: 0 };
        try {
            // If it's our pretty chart, get the BBox of the inner group to ignore pan/zoom viewport
            if (svgId === 'pretty-chart') {
                const innerG = svgElement.querySelector('g');
                if (innerG) {
                    bbox = innerG.getBBox();
                } else {
                    bbox = svgElement.getBBox();
                }
            } else {
                bbox = svgElement.getBBox();
            }
        } catch (e) {}

        const padding = 20;
        const width = bbox.width + padding * 2;
        const height = bbox.height + padding * 2;
        const minX = bbox.x - padding;
        const minY = bbox.y - padding;

        if (width <= padding * 2 || height <= padding * 2) return;

        const clonedSvg = svgElement.cloneNode(true);

        // Remove transform from inner G so pan/zoom doesn't affect the image
        if (svgId === 'pretty-chart') {
            const innerG = clonedSvg.querySelector('g');
            if (innerG) {
                innerG.removeAttribute('transform');
            }

            // Remove foreign objects (like inline editors) to prevent tainted canvas errors
            const foreignObjects = clonedSvg.querySelectorAll('foreignObject');
            foreignObjects.forEach(fo => fo.remove());
        }

        clonedSvg.setAttribute('width', width);
        clonedSvg.setAttribute('height', height);
        clonedSvg.setAttribute('viewBox', `${minX} ${minY} ${width} ${height}`);
        clonedSvg.setAttribute('xmlns', 'http://www.w3.org/2000/svg');

        // Inject styles
        const computedStyle = getComputedStyle(document.body);
        const mudPaletteSurface = computedStyle.getPropertyValue('--mud-palette-surface').trim() || '#252526';
        const mudPalettePrimary = computedStyle.getPropertyValue('--mud-palette-primary').trim() || '#569CD6';
        const mudPalettePrimaryLighten = computedStyle.getPropertyValue('--mud-palette-primary-lighten').trim() || '#7cb4e0';
        const mudPaletteTextPrimary = computedStyle.getPropertyValue('--mud-palette-text-primary').trim() || '#D4D4D4';

        const style = document.createElement('style');
        style.textContent = `
            :root {
                --mud-palette-surface: ${mudPaletteSurface};
                --mud-palette-primary: ${mudPalettePrimary};
                --mud-palette-primary-lighten: ${mudPalettePrimaryLighten};
                --mud-palette-text-primary: ${mudPaletteTextPrimary};
            }
            .sketch-font { font-family: 'Caveat', cursive; }
            svg { background-color: var(--mud-palette-surface); }
        `;
        clonedSvg.insertBefore(style, clonedSvg.firstChild);

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
