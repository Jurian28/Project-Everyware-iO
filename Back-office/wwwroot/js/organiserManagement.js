const loadingOrganisers = document.getElementById("loadingOrganisers");
const loadingRequests = document.getElementById("loadingRequests");
const loadingSpeakers = document.getElementById("loadingSpeakers");
const noOrganisersMessage = document.getElementById("noOrganisersMessage");
const noRequestsMessage = document.getElementById("noRequestsMessage");
const noSpeakersMessage = document.getElementById("noSpeakersMessage");

async function removeOrganiser(userId) {
    const response = await fetch(`/EventOrganiser/organiser/${userId}/revoke`, {
        method: "POST"
    });
    if (response.ok) {
        loadOrganisers();
    } else {
        console.error("Failed to remove organiser");
    }
}

async function removeSpeaker(userId) {
    const response = await fetch(`/EventOrganiser/speaker/${userId}/revoke`, {
        method: "POST"
    });
    if (response.ok) {
        loadSpeakers();
    } else {
        console.error("Failed to remove speaker");
    }
}

async function approveRequest(userId, role) {
    const response = await fetch(`/EventOrganiser/accept-request/${userId}?role=${encodeURIComponent(role)}`, {
        method: "POST"
    });
    if (response.ok) {
        loadRequests();
        loadOrganisers();
        loadSpeakers();
    } else {
        const text = await response.text().catch(() => 'Unknown error');
        console.error("Failed to approve request:", response.status, text);
        alert("Failed to approve request. Check console for details.");
    }
}

async function denyRequest(userId) {
    const response = await fetch(`/EventOrganiser/organiser/${userId}/deny`, {
        method: "POST"
    });
    if (response.ok) {
        loadRequests();
    } else {
        console.error("Failed to deny request");
    }
}

async function denyAllRequests() {
    const response = await fetch(`/EventOrganiser/organiser/deny/all`, {
        method: "POST"
    });
    if (response.ok) {
        loadRequests();
    } else {
        console.error("Failed to deny requests");
    }
}

async function loadOrganisers() {
    const response = await fetch(`/EventOrganiser/data/organisers`);
    const apiResponse = await response.json();
    const organisers = apiResponse.data;

    loadingOrganisers.classList.add("d-none");

    if (organisers && organisers.length > 0) {
        renderOrganisers(organisers);
        noOrganisersMessage.classList.add("d-none");
    } else {
        container = document.getElementById("organiserList");
        container.querySelectorAll(".organiser-row").forEach(el => el.remove());
        noOrganisersMessage.classList.remove("d-none");
    }
}

async function loadSpeakers() {
    const response = await fetch(`/EventOrganiser/data/Speakers`);
    const apiResponse = await response.json();
    const speakers = apiResponse.data;

    loadingSpeakers.classList.add("d-none");

    if (speakers && speakers.length > 0) {
        renderSpeakers(speakers);
        noSpeakersMessage.classList.add("d-none");
    } else {
        container = document.getElementById("speakerList");
        container.querySelectorAll(".speaker-row").forEach(el => el.remove());
        noSpeakersMessage.classList.remove("d-none");
    }
}

async function loadRequests() {
    const response = await fetch(`/EventOrganiser/data/Organisers-requests`);
    const apiResponse = await response.json();
    const requests = apiResponse.data;

    loadingRequests.classList.add("d-none");

    if (requests && requests.length > 0) {
        renderRequests(requests);
        noRequestsMessage.classList.add("d-none");
    } else {
        container = document.getElementById("requestList");
        container.querySelectorAll(".request-row").forEach(el => el.remove());
        noRequestsMessage.classList.remove("d-none");
    }
}

function renderOrganisers(organisers) {
    const container = document.getElementById("organiserList");
    const template = document.getElementById("organiserRowTemplate");

    container.querySelectorAll(".organiser-row").forEach(el => el.remove());

    organisers.forEach(organiser => {
        const clone = template.content.cloneNode(true);
        clone.querySelector("div").classList.add("organiser-row");
        clone.querySelector("[data-field='email']").textContent = organiser.email;
        clone.querySelector("[data-action='remove']").addEventListener("click", () => showModal("revokeOrganiserModal", () => removeOrganiser(organiser.id), organiser.email));
        container.appendChild(clone);
    });
}

function renderSpeakers(speakers) {
    const container = document.getElementById("speakerList");
    const template = document.getElementById("speakerRowTemplate");

    container.querySelectorAll(".speaker-row").forEach(el => el.remove());

    speakers.forEach(speaker => {
        const clone = template.content.cloneNode(true);
        clone.querySelector("div").classList.add("speaker-row");
        clone.querySelector("[data-field='email']").textContent = speaker.email;
        clone.querySelector("[data-action='remove']").addEventListener("click", () => showModal("revokeSpeakerModal", () => removeSpeaker(speaker.id), speaker.email));
        container.appendChild(clone);
    });
}

function renderRequests(requests) {
    const container = document.getElementById("requestList");
    const template = document.getElementById("requestRowTemplate");

    container.querySelectorAll(".request-row").forEach(el => el.remove());

    requests.forEach(request => {
        const clone = template.content.cloneNode(true);
        const row = clone.querySelector("div");
        const emailEl = clone.querySelector("[data-field='email']");
        const roleSelect = clone.querySelector("[data-action='role-select']");
        const approveBtn = clone.querySelector("[data-action='approve']");
        const denyBtn = clone.querySelector("[data-action='deny']");

        row.classList.add("request-row");
        emailEl.textContent = request.email;
        roleSelect.value = "Organiser";

        approveBtn.addEventListener("click", () => {
            approveRequest(request.id, roleSelect.value);
        });
        denyBtn.addEventListener("click", () => denyRequest(request.id));

        container.appendChild(clone);
    });
}

loadOrganisers();
loadSpeakers();
loadRequests();