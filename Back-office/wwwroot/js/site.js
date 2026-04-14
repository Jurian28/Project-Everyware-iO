
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
