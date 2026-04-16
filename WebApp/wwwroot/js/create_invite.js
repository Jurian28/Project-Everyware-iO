const createInviteForm = document.getElementById('createInviteForm');
const successMessageBox = document.getElementById('createInviteSuccessMessageBox');
const errorMessageBox = document.getElementById('createInviteErrorMessageBox');

function clearInviteMessages() {
    if (successMessageBox) {
        successMessageBox.innerText = '';
        successMessageBox.classList.add('d-none');
    }

    if (errorMessageBox) {
        errorMessageBox.innerText = '';
        errorMessageBox.classList.add('d-none');
    }
}

function showSuccessMessage(message) {
    if (!successMessageBox) {
        return;
    }

    successMessageBox.innerText = message;
    successMessageBox.classList.remove('d-none');
}

function showErrorMessage(message) {
    if (!errorMessageBox) {
        return;
    }

    errorMessageBox.innerText = message;
    errorMessageBox.classList.remove('d-none');
}

createInviteForm.addEventListener('submit', async event => {
    event.preventDefault();

    clearInviteMessages();

    const form = event.target;
    const formData = new FormData(form);
    const data = new URLSearchParams(formData);

    try {
        const response = await fetch(form.action, {
            method: 'POST',
            body: data
        });

        if (response.ok) {
            const modalElement = document.getElementById('createInviteModal');
            const modal = bootstrap.Modal.getInstance(modalElement);
            
            modal?.hide();

            const result = await response.json();
            const inviteLink = result.data?.inviteLink;

            if (inviteLink) {
                try {
                    await navigator.clipboard.writeText(inviteLink);
                    showSuccessMessage('Invite link copied to clipboard!');
                } catch (err) {
                    console.error('Failed to copy link: ', err);
                    showErrorMessage(`Failed to copy link. The link is: ${inviteLink}`);
                }
            } else {
                showSuccessMessage('Invite created successfully.');
            }
        } else {
            const errorText = await response.text();
            showErrorMessage(`Error creating invite: ${errorText}`);
        }
    } catch (error) {
        showErrorMessage(`Error creating invite: ${error.message}`);
    }
});

