export class AudioFile {
  
}

window.AudioFile = AudioFile;


window.getAudioDuration = function(url) {
    return new Promise((resolve) => {
        const audio = document.createElement('audio');
        audio.src = url;

        audio.addEventListener('loadedmetadata', () => {
            resolve(audio.duration);
        });

        audio.addEventListener('error', () => {
            resolve(''); 
        });

        audio.load();
    });
};