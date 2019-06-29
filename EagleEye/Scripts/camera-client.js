// JavaScript source code
function delay(t, v) {
    return new Promise(function (resolve) {
        setTimeout(resolve.bind(null, v), t)
    });
}
function begin() {
    const vid = document.querySelector('video');
    navigator.mediaDevices.getUserMedia({ video: true }) // request cam
        .then(stream => {
            vid.srcObject = stream; // don't use createObjectURL(MediaStream)
            return vid.play(); // returns a Promise
        })
        .then(() => {
            return delay(2000).then(function () {
                takeASnap()
                    .then(send);
            });
        })
        .catch(e => console.log('please use the fiddle instead'));
}
function takeASnap() {
    const canvas = document.createElement('canvas'); // create a canvas
    const ctx = canvas.getContext('2d'); // get its context
    canvas.width = vid.videoWidth; // set its size to the one of the video
    canvas.height = vid.videoHeight;
    ctx.drawImage(vid, 0, 0); // the video
    return new Promise((res, rej) => {
        canvas.toBlob(res, 'image/jpeg'); // request a Blob from the canvas
    });
}
function send(blob) {

}
function repeatingFunct() {
    //takeASnap().then(download);
    setTimeout(repeatingFunct, 5000)
}