const loadingMessage = document.getElementById("loadingMessage");
const noTagsMessage = document.getElementById("noTagsMessage");
const deleteButton = document.getElementById("deleteButton");
const editButtons = document.getElementsByClassName("js-edit-btn");

const eventId = parseInt(document.currentScript.dataset.eventId);

let selectedTag = null;

document.addEventListener("keydown", (e) => {
    if (e.key === "Escape") clearForm();
});

async function submitTag(e) {
    e.preventDefault();

    const tag = {
        idTag: selectedTag ? selectedTag.idTag : 0,
        idEvent: eventId,
        title: document.getElementById("title").value,
        colorHex: document.getElementById("colorHex").value
    };

    const response = await fetch(`/${eventId}/tag/data`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(tag)
    });

    if (response.ok) {
        showSuccess(`Saved tag: ${tag.title}`);
        clearForm();
        loadTags();
    } else {
        const err = (await response.json()).error;
        document.getElementById("errorMessageBox").innerText = err;
    }
}

async function deleteTag() {
    if (!selectedTag) return;

    await fetch(`/${eventId}/tag/${selectedTag.idTag}`, {
        method: "DELETE"
    });

    clearForm();
    loadTags();
}

function loadTagForEdit(tag) {
    selectedTag = tag;

    const tagRow = document.getElementById(`tag-${tag.idTag}`);
    if (tagRow) {
        unselectTagForEdit()
        tagRow.classList.add("bg-selected");
        tagRow.classList.remove("bg-white");
    }

    document.getElementById("title").value = tag.title;
    document.getElementById("colorHex").value = tag.colorHex ?? "";

    deleteButton.classList.remove("invisible");
    showEditButtons();
}

function clearForm() {
    selectedTag = null;
    unselectTagForEdit()
    document.getElementById("errorMessageBox").innerText = "";

    document.getElementById("title").value = "";
    document.getElementById("colorHex").value = "";

    deleteButton.classList.add("invisible");

    for (const b of editButtons) {
        b.classList.add("invisible");
    }
}

function showEditButtons() {
    for (const b of editButtons) {
        b.classList.remove("invisible");
    }
}

async function loadTags() {
    const response = await fetch(`/${eventId}/tag/data`);
    const api = await response.json();

    loadingMessage.classList.add("d-none");

    const tags = api.data;

    if (tags && tags.length > 0) {
        noTagsMessage.classList.add("d-none");
        renderTags(tags);
    } else {
        noTagsMessage.classList.remove("d-none");
    }
}

function renderTags(tags) {
    const container = document.getElementById("tagList");
    container.innerHTML = "";

    const template = document.getElementById("tagRowTemplate");

    tags.forEach(tag => {
        const clone = template.content.cloneNode(true);
        const row = clone.querySelector("div");
        row.id = `tag-${tag.idTag}`;

        clone.querySelector("[data-field='title']").textContent = tag.title;
        const colorEl = clone.querySelector("[data-field='color']");
        colorEl.style.backgroundColor = tag.colorHex || "#ccc";

        row.dataset.idTag = tag.idTag;

        row.addEventListener("click", () => loadTagForEdit(tag));

        container.appendChild(clone);
    });
}

function showSuccess(msg) {
    const box = document.getElementById("successMessageBox");
    box.innerText = msg;

    setTimeout(() => box.innerText = "", 4000);
}

function unselectTagForEdit() {
    const tagRows = document.querySelectorAll(`.bg-selected`);
    tagRows.forEach(tagRow => {
        tagRow.classList.remove("bg-selected");
        tagRow.classList.add("bg-white");
    });
}
loadTags();
