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
    
    // Check vertical position first
    if (top - tooltipHeight - margin < 0) {
        console.log('Not enough space above, placing below');
        top = elementRect.bottom;
        transform = 'translate(-50%, 0)';
        marginTop = margin;
    }
    
    // Check horizontal position with centered alignment
    const halfWidth = tooltipWidth / 2;
    let horizontalTransform = '-50%'; // Default: centered
    
    // Calculate desired tooltip position
    const desiredLeft = left - halfWidth;
    const desiredRight = left + halfWidth;
    
    if (desiredLeft < margin) {
        // Tooltip would go out of bounds on the left -> shift to the right
        //console.log('Shifting right: desiredLeft', desiredLeft, '< margin', margin);
        left = margin + halfWidth; // Center on the shifted position
        
        // Check if it goes out of bounds on the right
        if (left + halfWidth > elementRect.viewportWidth - margin) {
            // Tooltip is wider than viewport -> align left
            left = margin;
            horizontalTransform = '0';
        }
    } else if (desiredRight > elementRect.viewportWidth - margin) {
        // Tooltip would go out of bounds on the right -> shift to the left
        //console.log('Shifting left: desiredRight', desiredRight, '> viewport', elementRect.viewportWidth - margin);
        left = elementRect.viewportWidth - margin - halfWidth; // Center on the shifted position
        
        // Check if it goes out of bounds on the left
        if (left - halfWidth < margin) {
            // Tooltip is wider than viewport -> align left
            left = margin;
            horizontalTransform = '0';
        }
    }
    
    // Construct transform string
    const verticalTransform = transform.includes('-100%') ? '-100%' : '0';
    transform = `translate(${horizontalTransform}, ${verticalTransform})`;
    
    const result = {
        left: left,
        top: top,
        transform: transform,
        marginTop: marginTop
    };
    
    //console.log('Calculated position:', result);
    return result;
}

// Global mousemove handler as a fallback
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