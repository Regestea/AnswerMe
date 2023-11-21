

var GLOBAL = {};
GLOBAL.DotNetReference = null;
function setDotnetReference(pDotNetReference) {
    GLOBAL.DotNetReference = pDotNetReference;
}

window.addEventListener('resize', function () {
    const sizeCategory = getScreenSizeCategory();
    GLOBAL.DotNetReference.invokeMethodAsync( 'OnWindowResized', sizeCategory);
});

function getScreenSizeCategory() {
    const width = window.innerWidth;
    if (width < 640) {
        return "sm";
    } else if (width >= 640 && width < 768) {
        return "md";
    } else if (width >= 768 && width < 1024) {
        return "lg";
    } else if (width >= 1024 && width < 1280) {
        return "xl";
    } else {
        return "2xl";
    }
}




