const loadingOrganisers = document.getElementById("loadingOrganisers");
const loadingRequests = document.getElementById("loadingRequests");
const noOrganisersMessage = document.getElementById("noOrganisersMessage");
const noRequestsMessage = document.getElementById("noRequestsMessage");

async function removeOrganiser(userId) {
    const response = await fetch(`/auth/organiser/${userId}/revoke`, {
        method: "POST"
    });
    if (response.ok) {
        loadOrganisers();
    } else {
        console.error("Failed to remove organiser");
    }
}

async function approveRequest(userId) {
    const response = await fetch(`/auth/organiser/${userId}/accept`, {
        method: "POST"
    });
    if (response.ok) {
        loadRequests();
        loadOrganisers();
    } else {
        console.error("Failed to approve request");
    }
}

async function denyRequest(userId) {
    const response = await fetch(`/auth/organiser/${userId}/deny`, {
        method: "POST"
    });
    if (response.ok) {
        loadRequests();
    } else {
        console.error("Failed to deny request");
    }
}

async function denyAllRequests() {
    const response = await fetch(`/auth/organiser/deny/all`, {
        method: "POST"
    });
    if (response.ok) {
        loadRequests();
    } else {
        console.error("Failed to deny requests");
    }
}

async function loadOrganisers() {
    const response = await fetch(`/auth/data/organisers`);
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

async function loadRequests() {
    const response = await fetch(`/auth/data/Organisers-requests`);
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
        clone.querySelector("[data-action='remove']").addEventListener("click", () => showModal("revokeOrganiserModal" , () => removeOrganiser(organiser.id), organiser.email));
        container.appendChild(clone);
    });
}

function renderRequests(requests) {
    const container = document.getElementById("requestList");
    const template = document.getElementById("requestRowTemplate");

    container.querySelectorAll(".request-row").forEach(el => el.remove());

    requests.forEach(request => {
        const clone = template.content.cloneNode(true);
        clone.querySelector("div").classList.add("request-row");
        clone.querySelector("[data-field='email']").textContent = request.email;
        clone.querySelector("[data-action='approve']").addEventListener("click", () => approveRequest(request.id));
        clone.querySelector("[data-action='deny']").addEventListener("click", () => denyRequest(request.id));
        container.appendChild(clone);
    });
}

loadOrganisers();
loadRequests();