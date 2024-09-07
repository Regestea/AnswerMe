export function MessageToolsMenu(messageId) {
    chatMessageId = messageId;
    toggleMenu();
}

export function EnableScroll(){
    enableScroll();
}

let chatMessageId;

function toggleMenu() {
    const menuContent = document.getElementById('menuContent');
    const menuWidth = menuContent.offsetWidth;
    const menuHeight = menuContent.offsetHeight;

    // Get mouse position
    const mouseX = event.clientX;
    const mouseY = event.clientY;

    // Determine the position of the menu
    let menuX = mouseX;
    let menuY = mouseY;

    // Adjust the position if the menu is too close to the edges
    if (mouseX + menuWidth > window.innerWidth) {
        // Position the menu to the left of the mouse if it goes off the right edge
        menuX = mouseX - menuWidth;
    }
    if (mouseY + menuHeight > window.innerHeight) {
        // Position the menu above the mouse if it goes off the bottom edge
        menuY = mouseY - menuHeight;
    }

    if (isMouseCloseToRight(mouseX)) {
        menuContent.style.left = `${menuX - 230}px`;
    } else {
        menuContent.style.left = `${menuX}px`;
    }
    // Set the menu position
    menuContent.style.top = `${menuY}px`;

    // Toggle menu visibility
    if (
        menuContent.style.display === 'none' ||
        menuContent.style.display === ''
    ) {
        menuContent.style.display = 'flex';
        disableScroll();
    } else {
        menuContent.style.display = 'none';
        enableScroll();
    }
}



// Close the menu when clicking outside of it
document.onclick = function (event) {
    const menuContent = document.getElementById('menuContent');
    const chatElement = document.getElementById(chatMessageId);
    const chatContent = chatElement ? chatElement.querySelector('.chat-bubble') : null;
if (event.target !== null && chatContent !== null) {
    if (
        !menuContent.contains(event.target) &&
        !chatContent.contains(event.target)
    ) {
        menuContent.style.display = 'none';
        enableScroll();
    }
}

};

function isMouseCloseToRight(mouseX, threshold = 230) {
    // Get the width of the viewport
    const viewportWidth = window.innerWidth;

    // Check if the mouse is within the threshold distance from the right edge
    return mouseX > viewportWidth - threshold;
}


//disable & enable scroll

var keys = {37: 1, 38: 1, 39: 1, 40: 1};

function preventDefault(e) {
    e.preventDefault();
}

function preventDefaultForScrollKeys(e) {
    if (keys[e.keyCode]) {
        preventDefault(e);
        return false;
    }
}

var supportsPassive = false;
try {
    window.addEventListener("test", null, Object.defineProperty({}, 'passive', {
        get: function () { supportsPassive = true; }
    }));
} catch(e) {}

var wheelOpt = supportsPassive ? { passive: false } : false;
var wheelEvent = 'onwheel' in document.createElement('div') ? 'wheel' : 'mousewheel';

// Function to disable scroll
function disableScroll() {
    window.addEventListener('DOMMouseScroll', preventDefault, false); // older FF
    window.addEventListener(wheelEvent, preventDefault, wheelOpt); // modern desktop
    window.addEventListener('touchmove', preventDefault, wheelOpt); // mobile
    window.addEventListener('keydown', preventDefaultForScrollKeys, false);
}

// Function to enable scroll
function enableScroll() {
    window.removeEventListener('DOMMouseScroll', preventDefault, false);
    window.removeEventListener(wheelEvent, preventDefault, wheelOpt);
    window.removeEventListener('touchmove', preventDefault, wheelOpt);
    window.removeEventListener('keydown', preventDefaultForScrollKeys, false);
}
