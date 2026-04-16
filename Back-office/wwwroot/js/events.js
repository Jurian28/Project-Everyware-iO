
let toast = document.getElementById('event-toast');
if (toast) {
    setTimeout(closeToast, 4000);
}

const toastMessage = sessionStorage.getItem('eventToastMessage');
const toastType = sessionStorage.getItem('eventToastType');
if (toastMessage && toastType) {
    editToast(toastMessage, toastType);

    sessionStorage.removeItem('eventToastMessage');
    sessionStorage.removeItem('eventToastType');
}

function closeToast() {
    if (toast) {
        toast.classList.add('d-none');
        toast.classList.remove('d-flex');
    }
}

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

        sessionStorage.setItem('eventToastMessage', `Successfully ${published ? 'Published' : 'Unpublished'}!`);
        sessionStorage.setItem('eventToastType', 'success');

        window.location.reload();
    } catch (error) {
        console.error('Error:', error);
        editToast("Failed to publish event", "danger");
        throw error;
    }
}

function editToast(message, type) {
    if (toast) {
        toast.querySelector('p').textContent = message;

        toast.classList.remove('d-none');
        toast.classList.add('d-flex');
        toast.classList.remove('bg-success', 'bg-danger', 'bg-');
        toast.classList.add(`bg-${type}`);

        setTimeout(closeToast, 4000);
    }
}