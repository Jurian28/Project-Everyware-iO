async function togglePublish(eventId, published) {
    const url = `events/${eventId}/` + (published ? "unpublish" : "publish");
    try {
        const response = await fetch(url, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
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