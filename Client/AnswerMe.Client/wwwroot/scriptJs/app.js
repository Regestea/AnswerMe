
var GLOBAL = {};
GLOBAL.DotNetReference = null;

function setDotnetReference(pDotNetReference) {
    GLOBAL.DotNetReference = pDotNetReference;
}



window.addEventListener('resize', ResizeComponent);


function ResizeComponent() {
    const sizeCategory = getScreenSizeCategory();
    const isAtHome = IsAtHomePage();

    if (sizeCategory === 'sm' || sizeCategory === 'md' || sizeCategory === 'lg' || sizeCategory === 'xl') {

        if (isAtHome) {
            let mainBody = document.getElementById("ChatBody");
            let sidebar = document.getElementById("SidebarMenu");
            let accountMenu = document.getElementById("AccountMenu");
            
            if (mainBody !=null && sidebar !=null ){
                sidebar.classList.add('w-full');
                sidebar.classList.add('-translate-x-0');

                accountMenu.classList.add('w-full');
                accountMenu.classList.add('-translate-x-0');
                
                mainBody.classList.add('-translate-x-full');
                mainBody.classList.add('hidden');
            }
        }
        else {
            let sidebar = document.getElementById("SidebarMenu");
            let accountMenu = document.getElementById("AccountMenu");
            
            sidebar.classList.add('-translate-x-full');
            accountMenu.classList.add('-translate-x-full');
        }
        
    } else {
        let mainBody = document.getElementById("ChatBody");
        let sidebar = document.getElementById("SidebarMenu");
        let accountMenu = document.getElementById("AccountMenu");
        
        if (mainBody !=null && sidebar !=null){
            sidebar.classList.remove('-translate-x-0');
            sidebar.classList.remove('w-full');

            accountMenu.classList.remove('-translate-x-0');
            accountMenu.classList.remove('w-full');
            
            mainBody.classList.remove('-translate-x-full');
            mainBody.classList.remove('hidden');
        }
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



function MessageShow(messageType, messageHeader, messageText) {
    const modalHeader = document.getElementById("Modal-Message-Header");
    const modalText = document.getElementById("Modal-Text");
    const modal = document.getElementById("Modal-Message");

    switch (messageType) {
        case "info":
            modalHeader.className = "stat-value text-info";
            break;
        case "success":
            modalHeader.className = "stat-value text-accent";
            break;
        case "error":
            modalHeader.className = "stat-value text-secondary";
            break;
        default:
            modalHeader.className = "stat-value text-info";
            break;
    }

    modalHeader.innerText = messageHeader;
    modalText.innerText = messageText;

    modal.showModal();
}




