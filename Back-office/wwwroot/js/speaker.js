const loadingMessage = document.getElementById("loadingMessage");
const noSpeakersMessage = document.getElementById("noSpeakersMessage");
const deleteButton = document.getElementById("deleteButton");
const searchParams = new URLSearchParams(window.location.search);
const scriptTag = document.currentScript;
const eventId = parseInt(scriptTag.dataset.eventId);
const editButtons = document.getElementsByClassName("js-speaker-edit-button");

let idSpeaker = null;
$(document).keydown(function (e) {
    if (e.keyCode == 27) {
        clearSpeakerForm();
    }
});

async function submitSpeaker(event) {

    event.preventDefault();

    if (!event.target.checkValidity()) {
        event.target.reportValidity();
        return;
    }

    const speaker = {
        idSpeaker: idSpeaker ? parseInt(idSpeaker) : null,
        firstName: document.getElementById("firstName").value,
        middleName: document.getElementById("middleName").value,
        lastName: document.getElementById("lastName").value,
        description: document.getElementById("description").value,
        imgPath: document.getElementById("imgPath").value,
        idEvent: eventId,
    };

    const response = await fetch(`/${eventId}/speaker/data`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(speaker)
    });

    if (response.ok) {
        try {
            const isEditing = !!idSpeaker;
            document.getElementById("successMessageBox").innerText = `Successfully ${isEditing ? 'updated' : 'created'} a speaker with name: ${speaker.firstName} ${speaker.middleName} ${speaker.lastName}`;
            setTimeout(() => {
                document.getElementById("successMessageBox").innerText = "";
            }, 5000);
        } catch (e) {
            console.error(e)
        }
        clearSpeakerForm();
        loadSpeakers();
    } else {
        try {
            const errorResponse = await response.json();
            if (errorResponse && errorResponse.error) {
                document.getElementById("errorMessageBox").innerText = errorResponse.error;
            }
        }
        catch (e) {
            console.log("Error parsing error response", e);
        }
    }
}

async function deleteSpeaker() {
    if (!idSpeaker) return;
    const response = await fetch(`/${eventId}/speaker/${idSpeaker}`, {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
    });
    if (response.ok)
        clearSpeakerForm();
    loadSpeakers();
}

function clearSpeakerForm() {
    unselectSpeakerForEdit();
    idSpeaker = null;
    document.getElementById("speakerId").value = "";
    document.getElementById("firstName").value = "";
    document.getElementById("middleName").value = "";
    document.getElementById("lastName").value = "";
    document.getElementById("description").value = "";
    document.getElementById("imgPath").value = "";

    hideFormButtons();
}

function hideFormButtons() {
    deleteButton.classList.add("invisible");

    for (editButton of editButtons) {
        editButton.classList.add("invisible");
    }
}

function showEditButtons() {
    for (editButton of editButtons) {
        editButton.classList.remove("invisible");
    }
}

function loadSpeakerForEdit(speaker) {
    unselectSpeakerForEdit();
    idSpeaker = speaker.idSpeaker;
    const speakerRow = document.getElementById(`speaker-${idSpeaker}`);
    if (speakerRow) {
        speakerRow.classList.add("bg-selected");
        speakerRow.classList.remove("bg-white");
    }

    document.getElementById("firstName").value = speaker.firstName;
    document.getElementById("middleName").value = speaker.middleName ?? "";
    document.getElementById("lastName").value = speaker.lastName;
    document.getElementById("description").value = speaker.description ?? "";
    document.getElementById("imgPath").value = speaker.imgPath ?? "";
    deleteButton.classList.remove("invisible");
    showEditButtons();
}

function unselectSpeakerForEdit() {
    if (!idSpeaker) return;
    const speakerRow = document.getElementById(`speaker-${idSpeaker}`);
    speakerRow.classList.remove("bg-selected");
    speakerRow.classList.add("bg-white");
}

async function loadSpeakers() {
    const response = await fetch(`/${eventId}/speaker/data`);
    const apiResponse = await response.json();
    const speakers = apiResponse.data;
    loadingMessage.classList.add("d-none");
    if (speakers) {
        renderSpeakers(speakers)
    }
    else {
        noSpeakersMessage.classList.remove("d-none")
    }
}

function renderSpeakers(speakers) {
    const container = document.getElementById("speakerList");
    container.innerHTML = "";
    if (!speakers) return;

    const template = document.getElementById("speakerRowTemplate");
    container.innerHTML = "";

    speakers.forEach(speaker => {
        const clone = template.content.cloneNode(true);
        const row = clone.querySelector("div");
        row.id = `speaker-${speaker.idSpeaker}`;
        row.classList.add("bg-white");

        clone.querySelector("[data-field='speakerLabel']").textContent = speaker.speakerLabel;
        clone.querySelector("[data-field='capacity']").textContent = speaker.capacity;
        clone.querySelector("[data-field='description']").textContent = speaker.description ?? "";

        row.addEventListener("click", () => loadSpeakerForEdit(speaker));
        container.appendChild(clone);
    });
}

loadSpeakers();