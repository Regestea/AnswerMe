function CopyTextToClipboard(text) {
    navigator.clipboard.writeText(text).then(() => {
        console.log('Text copied to clipboard: ' + text);
    }).catch(err => {
        console.error('Failed to copy text:', err);
    });
}

function Click(elementId){
    let element=document.getElementById(elementId)
    if (element != null){
        element.click();
    }
}


async function GenerateVideoBase64Thumbnail(elementId) {
    const fileInput = document.getElementById(elementId);
    const file = fileInput.files[0];
    if (!file) {
        return null; // Return null if no file is selected
    }

    const video = document.createElement('video');
    video.src = URL.createObjectURL(file);

    // Create a promise to handle the video loading
    const loadPromise = new Promise((resolve) => {
        video.onloadeddata = () => resolve();
    });

    // Create a promise to handle the video seeking
    const seekPromise = new Promise((resolve) => {
        video.onseeked = () => resolve();
    });

    // Wait for the video to load and seek
    await loadPromise;
    video.currentTime = 1; // Capture frame at 1 second
    await seekPromise;

    const canvas = document.createElement('canvas');
    const context = canvas.getContext('2d');

    // Define target dimensions
    const targetWidth = 400;
    const targetHeight = 300;

    // Set canvas dimensions
    canvas.width = targetWidth;
    canvas.height = targetHeight;

    // Calculate the aspect ratio and scaling
    const scale = Math.min(targetWidth / video.videoWidth, targetHeight / video.videoHeight);
    const width = video.videoWidth * scale;
    const height = video.videoHeight * scale;
    const x = (targetWidth - width) / 2;
    const y = (targetHeight - height) / 2;

    // Draw the scaled video frame on the canvas
    context.drawImage(video, x, y, width, height);

    // Clean up
    URL.revokeObjectURL(video.src);

    return canvas.toDataURL('image/png');
}


function GenerateImageBase64(elementId,filePosition) {
    const fileInput = document.getElementById(elementId);
    
    const file = fileInput.files[filePosition];

    if (!file) {
        return '';
    }

    const reader = new FileReader();

    reader.onload = function(event) {
        const img = new Image();
        img.src = event.target.result;

        img.onload = function() {
            const canvas = document.createElement('canvas');
            const context = canvas.getContext('2d');

            // Define target dimensions
            const targetWidth = 400;
            const targetHeight = 300;

            // Calculate aspect ratio
            const aspectRatio = img.width / img.height;
            let width = targetWidth;
            let height = targetHeight;

            if (width / height > aspectRatio) {
                width = height * aspectRatio;
            } else {
                height = width / aspectRatio;
            }

            // Set canvas dimensions
            canvas.width = width;
            canvas.height = height;

            // Draw the resized image onto the canvas
            context.drawImage(img, 0, 0, width, height);

            // Convert canvas to Base64 PNG
            return canvas.toDataURL('image/png');
        };
    };
}


function GetInputFileType(elementId){
    let fileInput = document.getElementById(elementId);
    let selectedFile = fileInput.files[0];

    if (selectedFile) {
        return  selectedFile.name.split('.').pop();
    } else {
        return "";
    }
}

function CallVoidMethod(dotNetObjectReference,methodName){
   dotNetObjectReference.invokeMethodAsync(methodName);
}
function ScrollToView(id) {
    let element = document.getElementById(id);
    if (element != null) {
        element.scrollIntoView(
            // {behavior: 'smooth'}
        );
    }
}
function ScrollToEnd(id){
    var element = document.getElementById(id);
    element.scrollTo(0, element.scrollHeight);
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