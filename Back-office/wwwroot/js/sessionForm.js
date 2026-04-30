const startTime = document.querySelector('input[name="StartTime"]');
const endTime = document.querySelector('input[name="EndTime"]');
const plenaryToggle = document.getElementById('plenaryToggle');
const roomSelector = document.getElementById('roomSelector');
const capacityInput = document.getElementById('capacityInput');
const tagSelector = document.getElementById('tagSelector');
const tagContainer = document.getElementById('tagContainer');
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
function removeTag(tagDiv, title, colorHex, id) {
    const opt = document.createElement('option');
    opt.value = id;
    opt.text = title;
    opt.setAttribute('data-color', colorHex);
    tagSelector.add(opt);
    tagDiv.remove();
}

function addTag(title, colorHex, id) {
    if (!title) return;
    
    const tagDiv = document.createElement('div');
    tagDiv.className = "tag-item d-flex justify-content-between align-items-center mb-2 p-2 rounded border border-dark";
    tagDiv.style.backgroundColor = colorHex;
    tagDiv.style.color = contrastColor(colorHex);
    tagDiv.style.fontWeight = "600";

    tagDiv.innerHTML = `
                <span>${title}</span>
                <input type="hidden" name="selectedTagTitles" value="${title}" data-color="${colorHex}" />
                <i class="bi bi-x-circle-fill cursor-pointer" style="cursor:pointer"></i>
            `;

    tagDiv.querySelector('.bi-x-circle-fill').addEventListener('click', function () {
        removeTag(tagDiv, title, colorHex, id);
    });

    tagContainer.appendChild(tagDiv);

    for (let i = 0; i < tagSelector.options.length; i++) {
        if (tagSelector.options[i].value === id) {
            tagSelector.remove(i);
            break;
        }
    }
}

const existingHiddenInputs = tagContainer.querySelectorAll('input[name="selectedTagIds"]');
existingHiddenInputs.forEach(input => {
    const id = input.value;
    const colorHex = input.getAttribute('data-color');
    const title = input.getAttribute('data-title');
    addTag(title, colorHex, id);
});

tagSelector.addEventListener("change", function () {
    const id = this.value;
    const colorHex = this.options[this.selectedIndex].getAttribute("data-color");
    const title = this.options[this.selectedIndex].text;
    if (title && colorHex) {
        addTag(title,colorHex, id);
        this.value = "";
    }
});

function contrastColor(hexColor) {
    const r = parseInt(hexColor.slice(1, 3), 16);
    const g = parseInt(hexColor.slice(3, 5), 16);
    const b = parseInt(hexColor.slice(5, 7), 16);

    const luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;

    return luminance > 0.5 ? '#000000' : '#ffffff';
}