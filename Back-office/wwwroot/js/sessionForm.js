const startTime = document.querySelector('input[name="StartTime"]');
const endTime = document.querySelector('input[name="EndTime"]');
const plenaryToggle = document.getElementById('plenaryToggle');
const roomSelector = document.getElementById('roomSelector');
const capacityInput = document.getElementById('capacityInput');
const tagSelector = document.getElementById('tagSelector');
const tagContainer = document.getElementById('tagContainer');

const now = new Date();
now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
const nowString = now.toISOString().slice(0, 16);
startTime.min = nowString;

function updateEndTimeConstraint() {
    if (startTime.value) {
        endTime.min = startTime.value;

        if (!endTime.value || endTime.value < startTime.value) {
            endTime.value = startTime.value;
        }
    }
}

startTime.addEventListener("change", updateEndTimeConstraint);

if (startTime.value) {
    updateEndTimeConstraint();
}

function getSelectedRoomCapacity() {
    const selectedOption = roomSelector.options[roomSelector.selectedIndex];
    return selectedOption ? selectedOption.getAttribute('data-capacity') : null;
}

function handlePlenaryLogic() {
    const roomCapacity = getSelectedRoomCapacity();

    if (plenaryToggle.checked) {
        capacityInput.value = "";
        capacityInput.disabled = true;
        capacityInput.style.opacity = "0.2";
    } else {
        capacityInput.disabled = false;
        capacityInput.style.opacity = "1";
        if (roomCapacity && (capacityInput.value === "" || capacityInput.value === "0")) {
            capacityInput.value = roomCapacity;
        }
    }
}

roomSelector.addEventListener("change", function () {
    const roomCapacity = getSelectedRoomCapacity();
    if (roomCapacity && !plenaryToggle.checked) {
        capacityInput.value = roomCapacity;
    }
});

plenaryToggle.addEventListener("change", handlePlenaryLogic);
handlePlenaryLogic();

function removeTag(tagDiv, title) {
    const opt = document.createElement('option');
    opt.value = title;
    opt.text = title;
    tagSelector.add(opt);
    tagDiv.remove();
}

function addTag(title) {
    if (!title) return;

    const tagDiv = document.createElement('div');
    tagDiv.className = "tag-item d-flex justify-content-between align-items-center mb-2 p-2 rounded border border-dark";
    tagDiv.style.backgroundColor = "#E2F37C";
    tagDiv.style.fontWeight = "600";

    tagDiv.innerHTML = `
                <span>${title}</span>
                <input type="hidden" name="selectedTagTitles" value="${title}" />
                <i class="bi bi-x-circle-fill cursor-pointer" style="cursor:pointer"></i>
            `;

    tagDiv.querySelector('.bi-x-circle-fill').addEventListener('click', function () {
        removeTag(tagDiv, title);
    });

    tagContainer.appendChild(tagDiv);

    for (let i = 0; i < tagSelector.options.length; i++) {
        if (tagSelector.options[i].value === title) {
            tagSelector.remove(i);
            break;
        }
    }
}

const existingHiddenInputs = tagContainer.querySelectorAll('input[name="selectedTagTitles"]');
existingHiddenInputs.forEach(input => {
    const title = input.value;
    input.parentElement.remove();
    addTag(title);
});

tagSelector.addEventListener("change", function () {
    const title = this.value;
    if (title) {
        addTag(title);
        this.value = "";
    }
});