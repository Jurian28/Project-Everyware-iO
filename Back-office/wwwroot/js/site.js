
const toggle = document.getElementById('userDropdownToggle');
const icon = document.getElementById('dropDownToggleIcon');
const menu = document.getElementById('userDropdownMenu');

if (toggle && menu) {
    toggle.addEventListener('click', function (e) {
        e.stopPropagation();
        toggle.getAttribute('aria-expanded') === 'true' ? closeMenu() : openMenu();
    });

    document.addEventListener('click', function (e) {
        if (!document.getElementById('userDropdown').contains(e.target)) {
            closeMenu();
        }
    });
    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') closeMenu();
    });
}

function openMenu() {
    menu.classList.add('d-block');
    toggle.setAttribute('aria-expanded', true);
    icon.setAttribute('src', "../img/icons/chevron-up.svg");
    icon.setAttribute('alt', "arrow up");
}

function closeMenu() {
    menu.classList.remove('d-block');
    toggle.setAttribute('aria-expanded', false)
    icon.setAttribute('src', "../img/icons/chevron-down.svg");
    icon.setAttribute('alt', "arrow down");
}


let toast = document.getElementById('toast');
if (toast) {
    setTimeout(closeToast, 4000);
}

const toastMessage = sessionStorage.getItem('ToastMessage');
const toastType = sessionStorage.getItem('ToastType');
if (toastMessage && toastType) {
    editToast(toastMessage, toastType);

    sessionStorage.removeItem('ToastMessage');
    sessionStorage.removeItem('ToastType');
}

function closeToast() {
    if (toast) {
        toast.classList.add('d-none');
        toast.classList.remove('d-flex');
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