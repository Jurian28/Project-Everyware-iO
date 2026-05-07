function showModal(modalId, onYes, variableText) {
    const nameEl = document.getElementById(modalId + '-name');
    const yesBtn = document.getElementById(modalId + '-yes');

    if (variableText) {
        nameEl.textContent = variableText;
        nameEl.style.display = '';
    } else {
        nameEl.textContent = '';
        nameEl.style.display = 'none';
    }

    yesBtn.onclick = () => {
        bootstrap.Modal.getInstance(document.getElementById(modalId)).hide();
        if (typeof onYes === 'function') onYes();
    };

    new bootstrap.Modal(document.getElementById(modalId)).show();
}