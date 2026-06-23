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

function showInviteToken(token) {
    const form = document.getElementById('createInviteForm');
    const footer = document.querySelector('#createInviteModal .modal-footer');
    const template = document.getElementById('inviteTokenTemplate');

    if (form) form.classList.add('d-none');

    if (successMessageBox && template) {
        const content = template.content.cloneNode(true);
        content.querySelector('.js-inviteTokenDisplay').innerText = token;

        const copyButton = content.querySelector('.js-copyInviteToken');
        copyButton.addEventListener('click', () => copyInviteToken(token, copyButton));

        successMessageBox.innerHTML = '';
        successMessageBox.append(content);
        successMessageBox.classList.remove('d-none');
    }

    if (footer) {
        footer.innerHTML = '<button type="button" class="btn btn-primary" data-bs-dismiss="modal" onclick="resetInviteForm()">Close</button>';
    }
}

function copyInviteToken(token, btn) {
    navigator.clipboard.writeText(token).catch(() => {});
    if (btn) { btn.textContent = 'Copied!'; setTimeout(() => { btn.textContent = 'Copy'; }, 2000); }
}

function resetInviteForm() {
    const form = document.getElementById('createInviteForm');
    if (form) { form.classList.remove('d-none'); form.reset(); }
}

async function getInviteErrorMessage(response) {
    const errorMessage = 'Unable to create invite.';

    try {
        const result = await response.clone().json();
        if (typeof result?.error === 'string' && result.error.trim() !== '') {
            return result.error;
        }
    } catch (jsonError) {
        return errorMessage;
    }

    try {
        const errorText = await response.text();
        if (errorText.trim() !== '') {
            return errorText;
        }
    } catch (textError) {
        return errorMessage;
    }

    return errorMessage;
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
            const result = await response.json();
            const token = result.data?.token;

            if (token) {
                showInviteToken(token);
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

