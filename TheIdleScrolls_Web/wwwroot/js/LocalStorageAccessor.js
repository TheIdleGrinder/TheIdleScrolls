export function get(key) {
    return window.localStorage.getItem(key);
}

export function set(key, value) {
    window.localStorage.setItem(key, value);
}

export function clear() {
    window.localStorage.clear();
}

export function remove(key) {
    window.localStorage.removeItem(key);
}

export function downloadText(filename, content) {
    // Copied from https://www.meziantou.net/generating-and-downloading-a-file-in-a-blazor-webassembly-application.htm
    // Create the URL
    const file = new File([content], filename, { type: "text/plain" });
    const exportUrl = URL.createObjectURL(file);

    // Create the <a> element and click on it
    const a = document.createElement("a");
    document.body.appendChild(a);
    a.href = exportUrl;
    a.download = filename;
    a.target = "_self";
    a.click();

    // We don't need to keep the object URL, let's release the memory
    // On older versions of Safari, it seems you need to comment this line...
    URL.revokeObjectURL(exportUrl);
}