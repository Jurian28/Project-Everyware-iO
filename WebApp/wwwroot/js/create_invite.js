document.getElementById('createInviteForm').addEventListener('submit', function (event) {
    event.preventDefault();

    const form = event.target;
    const formData = new FormData(form);
    const data = new URLSearchParams(formData);

    fetch(form.action, {
        method: 'POST',
        body: data
    })
        .then(async response => {
            if (response.ok) {
                const modalElement = document.getElementById('createInviteModal');
                const modal = bootstrap.Modal.getInstance(modalElement);

                if (modal) {
                    modal.hide();
                }

                alert('Invite created successfully!');

                location.reload();
            } else {
                const errorText = await response.text();
                alert('Error creating invite: ' + errorText);
            }
        })
        .catch(error => {
            alert('Error creating invite: ' + error.message);
        });
});