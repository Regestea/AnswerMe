// JavaScript for the Image Slider

export function InitializeImageSlider(imageCount) {
    n_img = imageCount;
    corrente = 1;
    AddEventListeners();
    showImage(corrente);
}

export function Dispose(){
    RemoveEventListeners();
}

var n_img;
var corrente = 1;

// Function to show a specific image
function showImage(index) {
    for (var i = n_img; i > 0; i--) {
        document.querySelector("#img_slider img:nth-child(" + i + ")").style.display = "none";
    }
    document.querySelector("#img_slider img:nth-child(" + index + ")").style.display = "block";
}

// Function to go to the next image
function nextImage() {
    corrente++;
    if (corrente > n_img) {
        corrente = 1;
    }
    showImage(corrente);
}

// Function to go to the previous image
function previousImage() {
    corrente--;
    if (corrente === 0) {
        corrente = n_img;
    }
    showImage(corrente);
}

// Function to download the current image
function downloadImage() {
    var currentImage = document.querySelector("#img_slider img:nth-child(" + corrente + ")");
    if (currentImage) {
        window.open(currentImage.src, '_blank'); // Open the image in a new tab
    }
}

// Function to view the current image in fullscreen
function fullscreenImage() {
    var currentImage = document.querySelector("#img_slider img:nth-child(" + corrente + ")");
    if (currentImage) {
        if (currentImage.requestFullscreen) {
            currentImage.requestFullscreen();
        } else if (currentImage.mozRequestFullScreen) { // Firefox
            currentImage.mozRequestFullScreen();
        } else if (currentImage.webkitRequestFullscreen) { // Chrome, Safari and Opera
            currentImage.webkitRequestFullscreen();
        } else if (currentImage.msRequestFullscreen) { // IE/Edge
            currentImage.msRequestFullscreen();
        }
    }
}

// Function to add event listeners for slider controls and new buttons
function AddEventListeners() {
    document.getElementById("ImageSliderNextButton").addEventListener("click", nextImage);
    document.getElementById("ImageSliderPreviousButton").addEventListener("click", previousImage);

    // Add event listeners for the new buttons
    document.getElementById("ImageSliderDownloadImage").addEventListener("click", downloadImage);
    document.getElementById("ImageSliderFullscreenImage").addEventListener("click", fullscreenImage);
}

function RemoveEventListeners() {
    document.getElementById("ImageSliderNextButton").removeEventListener("click", nextImage);
    document.getElementById("ImageSliderPreviousButton").removeEventListener("click", previousImage);

    // Add event listeners for the new buttons
    document.getElementById("ImageSliderDownloadImage").removeEventListener("click", downloadImage);
    document.getElementById("ImageSliderFullscreenImage").removeEventListener("click", fullscreenImage);
}
