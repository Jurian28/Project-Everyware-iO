function requestPermission() {
    fetch("/Auth/Request-Permission", { method: 'POST' })
        .then(response => {
            if (!response.ok) return;
            document.getElementById("request").classList.toggle("d-none");
            document.getElementById("requested").classList.toggle("d-none");
            document.getElementById("requestAccessButton").classList.toggle("d-none");
        })
        .catch(error => {
            console.error("Error while requesting permission:", error);
        });
}