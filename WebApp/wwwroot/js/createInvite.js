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

async function getInviteErrorMessage(response) {
    try {
        const result = await response.clone().json();
        if (typeof result?.error === 'string' && result.error.trim() !== '') {
            return result.error;
        }
    } catch (jsonError) {
        // Fall back to text parsing below.
    }

    try {
        const errorText = await response.text();
        if (errorText.trim() !== '') {
            return errorText;
        }
    } catch (textError) {
        // Fall back to the default message below.
    }

    return 'Unable to create invite.';
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
            const errorMessage = await getInviteErrorMessage(response);
            showErrorMessage(`Error creating invite: ${errorMessage}`);
        }
    } catch (error) {
        showErrorMessage(`Error creating invite: ${error.message}`);
    }
});

