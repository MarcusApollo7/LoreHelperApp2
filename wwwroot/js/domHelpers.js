window.getBoundingClientRect = (element) => {
    if (!element) return null;
    const r = element.getBoundingClientRect();
    const result = {
        left: r.left,
        top: r.top,
        width: r.width,
        height: r.height
    };
    console.log('getBoundingClientRect:', {left: r.left, top: r.top, width: r.width, height: r.height});
    return result;
};

window.setTreeTransform = (offsetX, offsetY, zoom) => {
    console.log('setTreeTransform called:', {offsetX, offsetY, zoom});

    const viewport = document.querySelector('.tree-viewport');
    const transform = `translate(${offsetX}px, ${offsetY}px) scale(${zoom})`;
    console.log('Transform:', transform);

    if (viewport) {
        viewport.style.transform = transform;
        viewport.style.transformOrigin = '0 0';
        viewport.style.willChange = 'transform';
        console.log('Updated tree-viewport transform');
    } else {
        console.warn('tree-viewport not found');
    }
};

window.updateSVGLines = (childLinks, partnerLinks) => {
    console.log('updateSVGLines called:', {childLinks: childLinks.length, partnerLinks: partnerLinks.length});
    
    const svgG = document.querySelector('.tree-svg g');
    if (!svgG) {
        console.warn('SVG g not found');
        return;
    }
    
    // Remove all existing lines
    const existingLines = svgG.querySelectorAll('line');
    existingLines.forEach(line => line.remove());
    
    // Add child links (black lines)
    childLinks.forEach(link => {
        const line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
        line.setAttribute('x1', link.x1);
        line.setAttribute('y1', link.y1);
        line.setAttribute('x2', link.x2);
        line.setAttribute('y2', link.y2);
        line.setAttribute('stroke', '#000');
        line.setAttribute('stroke-width', '1.5');
        line.setAttribute('vector-effect', 'non-scaling-stroke');
        line.setAttribute('stroke-linecap', 'round');
        svgG.appendChild(line);
    });
    
    // Add partner links (red dashed lines)
    partnerLinks.forEach(link => {
        const line = document.createElementNS('http://www.w3.org/2000/svg', 'line');
        line.setAttribute('x1', link.x1);
        line.setAttribute('y1', link.y1);
        line.setAttribute('x2', link.x2);
        line.setAttribute('y2', link.y2);
        line.setAttribute('stroke', 'red');
        line.setAttribute('stroke-width', '2');
        line.setAttribute('stroke-dasharray', '4 2');
        line.setAttribute('vector-effect', 'non-scaling-stroke');
        svgG.appendChild(line);
    });
    
    console.log('SVG lines updated');
};

window.initTreePanZoom = (container) => {
    console.log('initTreePanZoom called');
    if (!container) {
        console.warn('initTreePanZoom: container is null');
        return;
    }

    let zoom = 1;
    let offsetX = 0;
    let offsetY = 0;
    let isDragging = false;
    let startX = 0;
    let startY = 0;

    let rafId = null;
    const apply = () => {
        if (rafId !== null) return;
        rafId = window.requestAnimationFrame(() => {
            window.setTreeTransform(offsetX, offsetY, zoom);
            rafId = null;
        });
    };

    const clampZoom = (value) => Math.max(0.3, Math.min(3.0, value));

    const onWheel = (evt) => {
        evt.preventDefault();

        const rect = container.getBoundingClientRect();
        const mouseX = evt.clientX - rect.left;
        const mouseY = evt.clientY - rect.top;

        const oldZoom = zoom;
        zoom = clampZoom(zoom + (evt.deltaY < 0 ? 0.1 : -0.1));

        const worldX = (mouseX - offsetX) / oldZoom;
        const worldY = (mouseY - offsetY) / oldZoom;

        offsetX = mouseX - worldX * zoom;
        offsetY = mouseY - worldY * zoom;

        apply();
    };

    const onMouseDown = (evt) => {
        isDragging = true;
        startX = evt.clientX - offsetX;
        startY = evt.clientY - offsetY;
        container.style.cursor = 'grabbing';
    };

    const onMouseUp = () => {
        isDragging = false;
        container.style.cursor = 'grab';
    };

    const onMouseMove = (evt) => {
        if (!isDragging) return;
        offsetX = evt.clientX - startX;
        offsetY = evt.clientY - startY;
        apply();
    };

    container.addEventListener('wheel', onWheel, { passive: false });
    container.addEventListener('mousedown', onMouseDown);
    window.addEventListener('mouseup', onMouseUp);
    window.addEventListener('mousemove', onMouseMove);

    // set initial transform
    apply();
};

window.testTransform = () => {
    console.log('=== TESTING TRANSFORM ===');
    window.setTreeTransform(100, 100, 1.5);
    
    setTimeout(() => {
        window.debugViewport();
    }, 100);
};

window.debugViewport = () => {
    const viewport = document.querySelector('.tree-viewport');
    const svg = document.querySelector('.tree-svg');
    const svgG = document.querySelector('.tree-svg g');
    const content = document.querySelector('.tree-content');
    
    console.log('=== DOM STRUCTURE ===');
    console.log('Viewport found:', !!viewport);
    console.log('SVG found:', !!svg);
    console.log('SVG g found:', !!svgG);
    console.log('Content found:', !!content);
    
    if (viewport) {
        console.log('Viewport:', {
            tagName: viewport.tagName,
            className: viewport.className,
            computed: {
                position: getComputedStyle(viewport).position,
                width: getComputedStyle(viewport).width,
                height: getComputedStyle(viewport).height
            }
        });
    }
    if (svg) {
        console.log('SVG:', {
            tagName: svg.tagName,
            className: svg.className,
            children: svg.children.length,
            transform: svg.getAttribute('transform'),
            computed: {
                position: getComputedStyle(svg).position,
                transform: getComputedStyle(svg).transform
            }
        });
    }
    if (svgG) {
        console.log('SVG g:', {
            tagName: svgG.tagName,
            transform: svgG.getAttribute('transform'),
            computed: {
                transform: getComputedStyle(svgG).transform
            }
        });
    }
    if (content) {
        console.log('Content:', {
            tagName: content.tagName,
            className: content.className,
            styleTransform: content.style.transform,
            computed: {
                position: getComputedStyle(content).position,
                transform: getComputedStyle(content).transform,
                transformOrigin: getComputedStyle(content).transformOrigin
            }
        });
    }
};