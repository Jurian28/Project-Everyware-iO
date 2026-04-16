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

                const result = await response.json();
                const inviteLink = result.data?.inviteLink;

                if (inviteLink) {
                    navigator.clipboard
                        .writeText(inviteLink)
                        .then(() => {
                            alert('Invite link copied to clipboard!');
                        })
                        .catch(err => {
                            console.error('Failed to copy link: ', err);
                            alert('Failed to copy link. The link is: ' + inviteLink);
                        });
                }
            } else {
                const errorText = await response.text();
                alert('Error creating invite: ' + errorText);
            }
        })
        .catch(error => {
            alert('Error creating invite: ' + error.message);
        });
});
