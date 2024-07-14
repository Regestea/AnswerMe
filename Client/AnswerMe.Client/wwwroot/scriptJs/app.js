var GLOBAL = {};
GLOBAL.DotNetReference = null;
GLOBAL.SetDotnetReference = function (pDotNetReference) {
    GLOBAL.DotNetReference = pDotNetReference;
};
GLOBAL.NavMenuDotNetReference = null;
GLOBAL.SetNavMenuDotnetReference = function (pDotNetReference) {
    GLOBAL.NavMenuDotNetReference = pDotNetReference;
};

function RefreshNavMenuData() {
    GLOBAL.NavMenuDotNetReference.invokeMethodAsync('RefreshNavMenuData');
}

//#region Save And Restore Chat Position

const SavedScrollPosition = {
    scrollTop: 0,
    scrollHeight: 0,
    clientHeight: 0
};

function SaveChatLocation(elementId) {
    let element = document.getElementById(elementId);
    
    if (element != null) {
        SavedScrollPosition.scrollTop = element.scrollTop;
        SavedScrollPosition.scrollHeight = element.scrollHeight;
        SavedScrollPosition.clientHeight = element.clientHeight;
    }
}

function RestoreChatLocation(elementId) {
    let element = document.getElementById(elementId);
    if (element != null) {
        let newScrollPosition = {
            scrollTop: element.scrollTop,
            scrollHeight: element.scrollHeight,
            clientHeight: element.clientHeight
        };
        let scrollTop=SavedScrollPosition.scrollTop+(newScrollPosition.scrollHeight - SavedScrollPosition.scrollHeight);
        element.scrollTop=scrollTop;
    }
}

//#endregion


// window.addEventListener('hashchange', () => {
//     console.log('Hash changed! New URL:', window.location.href);
// });
// window.addEventListener('popstate', (event) => {
//     console.log('URL changed! New URL:', window.location.href);
// });
function GetScrollStatus(elementId) {
    let element = document.getElementById(elementId);

    if (element != null) {
        return {
            ScrollTop: element.scrollTop,
            ScrollLeft: element.scrollLeft,
            ScrollHeight: element.scrollHeight,
            ClientHeight: element.clientHeight
        };
    }
    return null;
}

function SetScrollListenerLockStatus(scrollLock) {
    ScrollLock = scrollLock;
}

var ScrollLock = true;

function RegisterChatScrollListener(elementId) {

    let element = document.getElementById(elementId);
    if (element != null) {
        element.addEventListener('scroll', function () {
            ChatScroll(elementId)
        });
    }

};

function UnRegisterChatScrollListener(elementId) {
    let element = document.getElementById(elementId);
    if (element != null) {
        element.removeEventListener('scroll', function () {
            ChatScroll(elementId)
        });
    }
}

//fix this !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

var AllowLoadNewMessage = true;

function DisableLoadNewMessage() {
    AllowLoadNewMessage = false;
}

var AllowLoadOldMessage = true;

function DisableLoadOldMessage() {
    AllowLoadOldMessage = false;
}

var IsLoadMessageLocked = false;




function ChatScroll(elementId) {

    if (AllowLoadNewMessage === false && AllowLoadOldMessage === false) {
        return;
    }

    let scrollStatus;
    if (!ScrollLock) {
        scrollStatus = GetScrollStatus(elementId);
    }

    if (scrollStatus != null) {
        let totalScroll = scrollStatus.ScrollHeight - scrollStatus.ClientHeight;
        let remainingOneThird = totalScroll * 0.1;
        let lastOneThird = scrollStatus.ScrollTop > (totalScroll - remainingOneThird);
        let firstOneThird = scrollStatus.ScrollTop < remainingOneThird;

        if (IsLoadMessageLocked === false) {
            if (firstOneThird) {
                GLOBAL.DotNetReference.invokeMethodAsync('LoadOldMessages');

                IsLoadMessageLocked = true;
            }

            if (lastOneThird) {
                GLOBAL.DotNetReference.invokeMethodAsync('LoadNewMessages');
                IsLoadMessageLocked = true;
            }
        }

        if (firstOneThird === false && lastOneThird === false) {
            IsLoadMessageLocked = false;
        }

    }
}


window.addEventListener('resize', ResizeComponent);

function ViewAccountMenu() {
    let accountMenu = document.getElementById("AccountMenu");
    accountMenu.classList.remove('-translate-x-full');
    accountMenu.classList.add('-translate-x-0');
}

function ResizeComponent() {

    const sizeCategory = getScreenSizeCategory();
    const isAtHome = IsAtHomePage();

    if (sizeCategory === 'sm' || sizeCategory === 'md' || sizeCategory === 'lg' || sizeCategory === 'xl') {

        if (isAtHome) {
            let mainBody = document.getElementById("ChatBody");
            let sidebar = document.getElementById("SidebarMenu");
            let accountMenu = document.getElementById("AccountMenu");

            if (mainBody != null && sidebar != null) {
                sidebar.classList.add('w-full');
                sidebar.classList.remove('-translate-x-full');
                sidebar.classList.add('-translate-x-0');

                accountMenu.classList.add('w-full');
                accountMenu.classList.add('-translate-x-0');

                mainBody.classList.add('-translate-x-full');
                mainBody.classList.add('hidden');
            }
        } else {
            let sidebar = document.getElementById("SidebarMenu");
            let accountMenu = document.getElementById("AccountMenu");
            let mainBody = document.getElementById("ChatBody");

            mainBody.classList.remove('-translate-x-full');
            mainBody.classList.remove('hidden');
            sidebar.classList.add('-translate-x-full');
            accountMenu.classList.add('-translate-x-full');
        }

    } else {
        let mainBody = document.getElementById("ChatBody");
        let sidebar = document.getElementById("SidebarMenu");
        let accountMenu = document.getElementById("AccountMenu");

        if (mainBody != null && sidebar != null) {
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


function ValidationMessageBox(validationErrors) {
    const modalBox = document.getElementById("modal-box");
    const modal = document.getElementById("Modal-Message");
    modalBox.innerHTML = "";

    validationErrors.forEach(function (error) {

        let newDiv = document.createElement("div");
        newDiv.innerText = error.field;
        newDiv.className = "mt-6 stat-value text-secondary";

        let newText = document.createElement("p");
        newText.innerText = error.error;
        newText.className = "mt-6 ml-4";

        modalBox.insertBefore(newText, modalBox.firstChild);
        modalBox.insertBefore(newDiv, modalBox.firstChild);
    });
    modal.showModal();
}


function MessageShow(messageType, messageHeader, messageText) {
    const modalBox = document.getElementById("modal-box");
    const modal = document.getElementById("Modal-Message");
    modalBox.innerHTML = "";
    let modalHeaderClass = "";

    switch (messageType.toLowerCase()) {
        case "info":
            modalHeaderClass = "mt-4 stat-value text-info";
            break;
        case "success":
            modalHeaderClass = "mt-4 stat-value text-accent";
            break;
        case "error":
            modalHeaderClass = "mt-4 stat-value text-secondary";
            break;
        default:
            modalHeaderClass = "mt-4 stat-value text-info";
            break;
    }


    let newDiv = document.createElement("div");
    newDiv.innerText = messageHeader;
    newDiv.className = modalHeaderClass;

    let newText = document.createElement("p");
    newText.innerText = messageText;
    newText.className = "mt-4 ml-4";

    modalBox.insertBefore(newText, modalBox.firstChild);
    modalBox.insertBefore(newDiv, modalBox.firstChild);


    modal.showModal();
}
