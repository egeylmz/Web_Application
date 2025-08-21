// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

document.addEventListener("DOMContentLoaded", function () {         //oturum süresi sayaç
    let duration = window.oturumKalanSaniye || 60;
    let timerSpan = document.getElementById("session-timer");

    function updateTimer() {
        if (duration > 0) {
            let dakika = Math.floor(duration / 60);
            let saniye = duration % 60;
            timerSpan.textContent = `${dakika}:${saniye.toString().padStart(2, '0')}`;
            duration--;
        } else {
            // Oturum sona erdiğinde popup göster ve 2sn sonra ana ekrana yönlendir
            var modal = document.getElementById("oturumSonuModal");
            if (modal) {
                modal.style.display = "flex";
            }
            setTimeout(function () {
                window.location.href = "/Account/Logout";
            }, 2000);
        }
    }

    if (timerSpan) {
        updateTimer();
        setInterval(updateTimer, 1000);
    }
});