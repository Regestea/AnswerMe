var GLOBAL = {};
GLOBAL.DotNetReference = null;
GLOBAL.SetDotnetReference = function (pDotNetReference) {
    GLOBAL.DotNetReference = pDotNetReference;
};


window.addEventListener('hashchange', () => {
    console.log('Hash changed! New URL:', window.location.href);
});
window.addEventListener('popstate', (event) => {
    console.log('URL changed! New URL:', window.location.href);
});

function ScrollToView(id) {
    let element = document.getElementById(id);
    if (element != null) {
        element.scrollIntoView(
            // {behavior: 'smooth'}
        );
    }
}

function AddClass(id, className) {
    let element = document.getElementById(id);
    if (element != null) {
        element.classList.add(className)
    }
}

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

function RemoveClass(id, className) {
    let element = document.getElementById(id);
    if (element != null) {
        element.classList.remove(className);
    }
}

function ReplaceClass(id, oldClassName, newClassName) {
    let element = document.getElementById(id);
    if (element != null) {
        element.classList.replace(oldClassName, newClassName);
    }
}

function SetInnerText(id, text) {
    let element = document.getElementById(id);
    if (element != null) {
        element.innerText = text;
    }
}

function ScrollToEnd(id){
    var element = document.getElementById(id);
    element.scrollTo(0, element.scrollHeight);
}

function SetScrollListenerLockStatus(scrollLock) {
    ScrollLock = scrollLock;
}

// this dosen't work because we doesn't scroll window , we scroll a div element so we need a function to register event listener for the div then on component dispose unRegister it
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

var IsLoadMessageLocked = false;

function ChatScroll(elementId) {

    let scrollStatus;
    if (!ScrollLock) {
        scrollStatus= GetScrollStatus(elementId);
    }

    if (scrollStatus != null) {

        let totalScroll = scrollStatus.ScrollHeight - scrollStatus.ClientHeight;
        let remainingOneThird = totalScroll / 10;
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

    switch (messageType) {
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
