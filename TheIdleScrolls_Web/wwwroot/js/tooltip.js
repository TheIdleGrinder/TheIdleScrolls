export function getElementPosition(element) {
    const rect = element.getBoundingClientRect();
    return {
        left: rect.left,
        top: rect.top,
        width: rect.width,
        height: rect.height,
        right: rect.right,
        bottom: rect.bottom
    };
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