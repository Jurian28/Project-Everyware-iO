const startDateInput = document.getElementById('start-date');
const endDateInput = document.getElementById('end-date');
const form = document.getElementById('eventForm');
form.addEventListener('submit', function (e) {
    e.preventDefault();
    if (startDateInput.value && endDateInput.value) {
        const startDate = new Date(startDateInput.value);
        const endDate = new Date(endDateInput.value);
        if (startDate > endDate) {
            editToast("Start date cannot be later than end date.", "danger");
            return;
        }
    }
    form.submit();
});