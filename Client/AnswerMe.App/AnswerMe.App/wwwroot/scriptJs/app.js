
var GLOBAL = {};
GLOBAL.DotNetReference = null;

function setDotnetReference(pDotNetReference) {
    GLOBAL.DotNetReference = pDotNetReference;
}

ResizeComponent();

window.addEventListener('resize', ResizeComponent);


function ResizeComponent() {
    const sizeCategory = getScreenSizeCategory();
    const isAtHome = IsAtHomePage();

    if (sizeCategory === 'sm' || sizeCategory === 'md' || sizeCategory === 'lg' || sizeCategory === 'xl') {

        if (isAtHome) {
            let mainBody = document.getElementById("MainBody");
            let sidebar = document.getElementById("SidebarMenu");
            sidebar.style.width = '100%';
            sidebar.classList.add('-translate-x-0');
            mainBody.classList.add('-translate-x-full');
        }
        
    } else {
        let mainBody = document.getElementById("MainBody");
        let sidebar = document.getElementById("SidebarMenu");
        sidebar.classList.remove('-translate-x-0');
        sidebar.style.width = '';
        mainBody.classList.remove('-translate-x-full');
    }
}

function IsAtHomePage() {
    // Check if the user is at the home page
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




