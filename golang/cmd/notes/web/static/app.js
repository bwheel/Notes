let timeout;
const textarea = document.getElementById("content");
const uuid = document.getElementById("uuid").value;
const countdown = document.getElementById("countdown");

textarea.addEventListener("input", () => {
    clearTimeout(timeout);
    timeout = setTimeout(() => {
        fetch("/s/" + uuid, {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: new URLSearchParams({ content: textarea.value })
        }).catch(err => console.error("Save failed:", err));
    }, 500);
});


// Countdown timer
const expireMinutes = 15;
let startTime = Date.now();

function updateTimer() {
    const elapsed = Math.floor((Date.now() - startTime) / 1000);
    const remaining = expireMinutes * 60 - elapsed;

    if (remaining <= 0) {
        countdown.textContent = "expired";
        countdown.style.color = "red";
        clearInterval(timerInterval);
        return;
    }

    const minutes = Math.floor(remaining / 60);
    const seconds = remaining % 60;
    countdown.textContent = `expires in ${minutes}:${seconds.toString().padStart(2, "0")}`;
}
updateTimer(); // initialize immediately
const timerInterval = setInterval(updateTimer, 1000);
