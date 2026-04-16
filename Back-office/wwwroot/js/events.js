// # Toast
let toast = document.getElementById('event-toast');
if (toast) {
    setTimeout(closeToast, 4000);
}

function closeToast() {
    if (toast) {
        toast.style.display = 'none';
        toast.classList.remove('d-flex');
    }
}

// # Publish
async function togglePublish(eventId, published) {
    const apiBaseUrl = document.getElementById('api-url').value;
    const url = `${apiBaseUrl}/api/event/${eventId}/publish`;

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                publish: published
            }),
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        window.location.reload();
    } catch (error) {
        console.error('Error:', error);
        throw error;
    }
}
