window.geauxFileDrop = {
    registerDropZone: function (element, dotNetRef) {
        element.addEventListener("dragover", function (e) {
            e.preventDefault();
            e.dataTransfer.dropEffect = "copy";
        });

        element.addEventListener("drop", function (e) {
            e.preventDefault();

            if (e.dataTransfer.files.length > 0) {
                const file = e.dataTransfer.files[0];
                window._geauxDroppedFile = file;

                dotNetRef.invokeMethodAsync("OnJsFileDropped", {
                    name: file.name,
                    size: file.size,
                    type: file.type,
                    lastModified: file.lastModified
                });
            }
        });
    },

    readChunk: async function (position, length) {
        const file = window._geauxDroppedFile;
        if (!file) return null;

        const blob = file.slice(position, position + length);
        const arrayBuffer = await blob.arrayBuffer();
        return new Uint8Array(arrayBuffer);
    }
};
