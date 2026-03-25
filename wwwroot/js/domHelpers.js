window.getBoundingClientRect = (element) => {
    if (!element) return null;
    const r = element.getBoundingClientRect();
    return {
        left: r.left,
        top: r.top,
        width: r.width,
        height: r.height
    };
};