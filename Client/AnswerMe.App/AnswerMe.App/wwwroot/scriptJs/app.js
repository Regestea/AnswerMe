//do all of them in js if user was at home page and the screen was too small full screen the side bar
var GLOBAL = {};
GLOBAL.DotNetReference = null;

function setDotnetReference(pDotNetReference) {
    GLOBAL.DotNetReference = pDotNetReference;
}

window.addEventListener('resize', function () {
    const sizeCategory = getScreenSizeCategory();
    const isAtHome=IsAtHomePage();
    document.getElementById("NavMenu");
    document.getElementById("MainBody");
});

function IsAtHomePage() {
    // Check if the user is on the home page
    return window.location.pathname === '/';
}

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




