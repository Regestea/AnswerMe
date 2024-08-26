export function LoadVideoPlayer() {
    initializeVideoPlayerElements();
    setVideoData();
    AddEventListeners();
}

export function DisposeVideoPlayer(){
    clearVideoPlayerElements();
    removeVideoPlayerEventListeners();
}

let controls, fullscreen, playButton, player, timeCounter, timeBar, video, volumeSlider;

function initializeVideoPlayerElements() {
    controls      = document.getElementById('VideoPlayer-controls');
    fullscreen    = document.getElementById('VideoPlayer-fullscreen');
    playButton    = document.getElementById('VideoPlayer-playButton');
    player        = document.getElementById('VideoPlayer-player');
    timeCounter   = document.getElementById('VideoPlayer-timeCounter');
    timeBar       = document.getElementById('VideoPlayer-timeBar');
    video         = document.getElementById('VideoPlayer-video');
    volumeSlider  = document.getElementById('VideoPlayer-volumeSlider');
}



    let isMouseDown = false,
        timeTotal = 0,
        uiTimeout = '',
        videoStatus = 'paused';

    function onKeyDown(e) {
        switch(e.key) {
            case 'ArrowLeft':
            case 'ArrowRight':
                skip(e);
                break;
            case 'ArrowUp':
            case 'ArrowDown':
                adjustVolume(e);
                break;
            case ' ':
                playVideo(e);
                break;
        }
        showUI();
    }

    function showUI() {
        if (uiTimeout) clearTimeout(uiTimeout);
        controls.classList.add('active');
        video.style.cursor = 'default';
    }

    function hideUI() {
        if (uiTimeout) clearTimeout(uiTimeout);
        if (video.paused) return;

        uiTimeout = setTimeout(() => {
            controls.classList.remove('active');
            setTimeout(() => video.style.cursor = 'none', 1000);
        }, 2000);
    }

    function onMouseDown() {
        isMouseDown = true;
        showUI();
    }

    function onMouseUp() {
        isMouseDown = false;
        if (videoStatus === 'paused') return;

        hideUI();
        video.play();
    }

    function updatePlayState() {
        video.paused ?
            (playButton.classList.add('start'), playButton.classList.remove('pause')) :
            (playButton.classList.add('pause'), playButton.classList.remove('start'));

        video.paused ? showUI() : hideUI();
    }

    const updatetimeBar = (e) => {
        if (!isMouseDown && e.type === 'mousemove') return;

        video.currentTime = timeBar.value;
        if (!isMouseDown) return;

        video.pause();
        showUI();
        hideUI();
    }

    function updateCurrentTime() {
        let seconds = Math.floor(video.currentTime % 60);
        let minutes = Math.floor(video.currentTime / 60);
        seconds = seconds >= 10 ? seconds : '0' + seconds;
        timeCounter.innerText = `${minutes}:${seconds} / ${timeTotal}`;
        if (isMouseDown) return;

        timeBar.value = video.currentTime;
    }

    function playVideo(e) {
        e.preventDefault();
        if (!video.readyState >= 2) return;

        if (video.paused) {
            video.play();
            videoStatus = 'playing';
        } else {
            video.pause();
            videoStatus = 'paused';
        }
    }

    function adjustVolume(e) {
        if (e.type === 'mousemove' && !isMouseDown) return;
        if (e.which === 1 ) return video.volume = volumeSlider.value / 100;

        e.preventDefault();
        if (e.key === 'ArrowUp' || e.wheelDelta > 0) {
            video.volume = video.volume + .1 >= 1 ? 1 : video.volume + .1;
            volumeSlider.value = video.volume * 100;
            return;
        }
        if (e.key === 'ArrowDown' || e.wheelDelta < 0) {
            video.volume = video.volume - .1 <= 0 ? 0 : video.volume - .1;
            volumeSlider.value = video.volume * 100;
            return;
        }

        video.volume = volumeSlider.value / 100;
    }

    function skip(e) {
        e.preventDefault();
        switch(e.key) {
            case 'ArrowLeft':
                video.currentTime -= 10;
                break;
            case 'ArrowRight':
                video.currentTime += 10;
                break;
        }
        timeBar.value = video.currentTime;
    }

    function toggleFullScreen() {
        if (!document.fullscreenElement &&
            !document.mozFullScreenElement &&
            !document.webkitFullscreenElement &&
            !document.msFullscreenElement) {
            if (player.requestFullscreen) {
                player.requestFullscreen();
            } else if (player.mozRequestFullScreen) {
                player.mozRequestFullScreen();
            } else if (player.webkitRequestFullscreen) {
                player.webkitRequestFullscreen();
            } else if (player.msRequestFullscreen) {
                player.msRequestFullscreen();
            }
        } else {
            if (document.exitFullscreen) {
                document.exitFullscreen();
            } else if (document.mozCancelFullScreen) {
                document.mozCancelFullScreen();
            } else if (document.webkitExitFullscreen) {
                document.webkitExitFullscreen();
            } else if (document.msExitFullscreen) {
                document.msExitFullscreen();
            }
        }

        setVideoSize();
    }

    function setVideoSize() {
        const aspectRatio = video.offsetWidth / video.offsetHeight;
        controls.style.width = player.offsetWidth + 'px';
        if (video.offsetHeight >= player.clientHeight) {
            video.style.width = window.innerHeight * aspectRatio + 'px';
        } else {
            video.style.width = player.offsetWidth + 'px';
        }
        const margin = (player.offsetHeight - video.offsetHeight) / 2 + 'px';
        video.style.marginTop = margin;
    }

    function setVideoData() {
        if (video.readyState) {
            let seconds = Math.floor(video.duration % 60);
            let minutes = Math.floor(video.duration / 60);
            seconds = seconds >= 10 ? seconds : '0' + seconds;
            timeTotal = `${minutes}:${seconds}`;
            timeBar.max = video.duration;
            timeCounter.innerText = `0:00 / ${timeTotal}`;
            updateCurrentTime();
        }
        video.volume = volumeSlider.value / 100;
        setVideoSize();
    }


function AddEventListeners(){
    controls.addEventListener('mousemove', () => { showUI(), hideUI() });
    controls.addEventListener('mouseout', hideUI);

    controls.childNodes.forEach(control => control.addEventListener('mousedown', onMouseDown));
    controls.childNodes.forEach(control => control.addEventListener('mouseup', onMouseUp));
    controls.childNodes.forEach(control => control.addEventListener('touchstart', onMouseDown));
    controls.childNodes.forEach(control => control.addEventListener('touchend', onMouseUp));

    fullscreen.addEventListener('click', toggleFullScreen);

    playButton.addEventListener('click', playVideo);
    player.addEventListener('fullscreenchange', setVideoSize);
    player.addEventListener('msfullscreenchange', setVideoSize);
    timeBar.addEventListener('change', updatetimeBar);
    timeBar.addEventListener('mousemove', updatetimeBar);
    video.addEventListener('mouseup', playVideo);
    video.addEventListener('touchend', playVideo);
    video.addEventListener('loadedmetadata', setVideoData);
    video.addEventListener('play', updatePlayState);
    video.addEventListener('pause', updatePlayState);
    video.addEventListener('timeupdate', updateCurrentTime);
    video.addEventListener('mouseout', hideUI);
    video.addEventListener('dblclick', toggleFullScreen);
    video.addEventListener('mousemove', () => { showUI(), hideUI() });

    volumeSlider.addEventListener('change', adjustVolume);
    volumeSlider.addEventListener('mousemove', adjustVolume);
    volumeSlider.addEventListener('wheel', adjustVolume);

    window.addEventListener('keydown', onKeyDown);
    window.addEventListener('keyup', hideUI);
    window.addEventListener('resize', setVideoSize);
    window.addEventListener('mouseup', onMouseUp);
}


function removeVideoPlayerEventListeners() {
    // Elements to remove event listeners from
    const controls     = document.getElementById('VideoPlayer-controls'),
        fullscreen     = document.getElementById('VideoPlayer-fullscreen'),
        playButton     = document.getElementById('VideoPlayer-playButton'),
        player         = document.getElementById('VideoPlayer-player'),
        timeBar        = document.getElementById('VideoPlayer-timeBar'),
        video          = document.getElementById('VideoPlayer-video'),
        volumeSlider   = document.getElementById('VideoPlayer-volumeSlider');
    
    const removeControlListeners = (element) => {
        element.removeEventListener('mousemove', showUI);
        element.removeEventListener('mouseout', hideUI);
        element.removeEventListener('mousedown', onMouseDown);
        element.removeEventListener('mouseup', onMouseUp);
        element.removeEventListener('touchstart', onMouseDown);
        element.removeEventListener('touchend', onMouseUp);
    };
    removeControlListeners(controls);
    controls.childNodes.forEach(control => removeControlListeners(control));
    
    fullscreen.removeEventListener('click', toggleFullScreen);
    playButton.removeEventListener('click', playVideo);
    player.removeEventListener('fullscreenchange', setVideoSize);
    player.removeEventListener('msfullscreenchange', setVideoSize);
    timeBar.removeEventListener('change', updatetimeBar);
    timeBar.removeEventListener('mousemove', updatetimeBar);
    video.removeEventListener('mouseup', playVideo);
    video.removeEventListener('touchend', playVideo);
    video.removeEventListener('loadedmetadata', setVideoData);
    video.removeEventListener('play', updatePlayState);
    video.removeEventListener('pause', updatePlayState);
    video.removeEventListener('timeupdate', updateCurrentTime);
    video.removeEventListener('mouseout', hideUI);
    video.removeEventListener('dblclick', toggleFullScreen);
    video.removeEventListener('mousemove', () => { showUI(), hideUI() });

    volumeSlider.removeEventListener('change', adjustVolume);
    volumeSlider.removeEventListener('mousemove', adjustVolume);
    volumeSlider.removeEventListener('wheel', adjustVolume);

    window.removeEventListener('keydown', onKeyDown);
    window.removeEventListener('keyup', hideUI);
    window.removeEventListener('resize', setVideoSize);
    window.removeEventListener('mouseup', onMouseUp);
}

function clearVideoPlayerElements() {
    controls      = null;
    fullscreen    = null;
    playButton    = null;
    player        = null;
    timeCounter   = null;
    timeBar       = null;
    video         = null;
    volumeSlider  = null;
}

