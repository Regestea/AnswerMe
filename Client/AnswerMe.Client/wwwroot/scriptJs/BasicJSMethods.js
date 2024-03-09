function ScrollToView(id) {
    let element = document.getElementById(id);
    if (element != null) {
        element.scrollIntoView(
            // {behavior: 'smooth'}
        );
    }
}

function SubmitForm(id) {
    let element = document.getElementById(id);
    if (element != null) {
        element.submit();
    }
}

function AddClass(id, className) {
    let element = document.getElementById(id);
    if (element != null) {
        element.classList.add(className)
    }
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