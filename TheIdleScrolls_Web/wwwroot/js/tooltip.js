export function getElementPosition(element) {
    const rect = element.getBoundingClientRect();
    return {
        left: rect.left,
        top: rect.top,
        width: rect.width,
        height: rect.height,
        right: rect.right,
        bottom: rect.bottom,
        viewportWidth: window.innerWidth,
        viewportHeight: window.innerHeight
    };
}

export function calculateTooltipPosition(elementRect, tooltipRect) {
    const margin = 10;
    let left = elementRect.left + (elementRect.width / 2);
    let top = elementRect.top;
    let transform = 'translate(-50%, -100%)';
    let marginTop = -margin;
    
    const tooltipWidth = tooltipRect.width;
    const tooltipHeight = tooltipRect.height;
    
    console.log('Element:', elementRect);
    console.log('Tooltip size:', tooltipWidth, 'x', tooltipHeight);
    console.log('Viewport:', elementRect.viewportWidth, 'x', elementRect.viewportHeight);
    
    // Prüfe vertikale Position
    if (top - tooltipHeight - margin < 0) {
        console.log('Not enough space above, placing below');
        top = elementRect.bottom;
        transform = 'translate(-50%, 0)';
        marginTop = margin;
    }
    
    // Prüfe horizontale Position mit zentrierter Ausrichtung
    const halfWidth = tooltipWidth / 2;
    let horizontalTransform = '-50%'; // Standard: zentriert
    
    // Berechne wo der Tooltip hinwill
    const desiredLeft = left - halfWidth;
    const desiredRight = left + halfWidth;
    
    if (desiredLeft < margin) {
        // Tooltip würde links rausgehen -> verschiebe nach rechts
        console.log('Shifting right: desiredLeft', desiredLeft, '< margin', margin);
        left = margin + halfWidth; // Zentriere auf der verschobenen Position
        
        // Prüfe ob dadurch rechts rausgeht
        if (left + halfWidth > elementRect.viewportWidth - margin) {
            // Tooltip ist breiter als Viewport -> linksbündig
            left = margin;
            horizontalTransform = '0';
        }
    } else if (desiredRight > elementRect.viewportWidth - margin) {
        // Tooltip würde rechts rausgehen -> verschiebe nach links
        console.log('Shifting left: desiredRight', desiredRight, '> viewport', elementRect.viewportWidth - margin);
        left = elementRect.viewportWidth - margin - halfWidth; // Zentriere auf der verschobenen Position
        
        // Prüfe ob dadurch links rausgeht
        if (left - halfWidth < margin) {
            // Tooltip ist breiter als Viewport -> linksbündig
            left = margin;
            horizontalTransform = '0';
        }
    }
    
    // Konstruiere Transform-String
    const verticalTransform = transform.includes('-100%') ? '-100%' : '0';
    transform = `translate(${horizontalTransform}, ${verticalTransform})`;
    
    const result = {
        left: left,
        top: top,
        transform: transform,
        marginTop: marginTop
    };
    
    console.log('Calculated position:', result);
    return result;
}

// Globaler Mousemove-Handler als Fallback
let lastMouseX = 0;
let lastMouseY = 0;

document.addEventListener('mousemove', (e) => {
    lastMouseX = e.clientX;
    lastMouseY = e.clientY;
});

export function isMouseOverElement(className) {
    const element = document.elementFromPoint(lastMouseX, lastMouseY);
    return element?.closest(`.${className}`) !== null;
}