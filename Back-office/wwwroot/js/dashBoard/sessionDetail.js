import { getConfig, upsert } from '../chartHelper.js';

// --- 1. Haal elementen uit de HTML ---
const canvas = document.getElementById("sessionChart");
const noChartElement = document.getElementById("noChart");

const statPresent = document.getElementById("stat-presentCount");
const statAbsent = document.getElementById("stat-absentCount");
const statOpenSpots = document.getElementById("stat-openSpots");
const statTotalSpots = document.getElementById("stat-totalSpots");
const statWaitlist = document.getElementById("stat-waitlist");

const listContainer = document.getElementById("attendeeListContainer");
const loadingElement = document.getElementById("loadingAttendees");
const attendeeTemplate = document.getElementById("attendeeCardTemplate");

const eventId = parseInt(document.querySelector('meta[name="event-id"]').content);
const sessionId = parseInt(document.querySelector('meta[name="session-id"]').content);
const chartConfig = getConfig({ cutout: "60%" });
let chart;

let session;

// TODO refactor name
initPage();
// setInterval(initPage, 5000);

async function initPage() {
    await fetchSessionDetails();

    if (!session || session.totalSpots === 0) {
        showNoData();
        return;
    }

    updateStats();
    updateChart();
    renderAttendeeList();
}

async function fetchSessionDetails() {
    const response = await fetch(`/${eventId}/session/GetSpotsData/${sessionId}`);
    const data = await response.json();
    if (data && data.length > 0) {
        session = data[0];
    }
}

function updateStats() {
    statTotalSpots.innerText = session.totalSpots;
    statPresent.innerText = 0; // TODO update when new db field added
    statAbsent.innerText = 0; // TODO update when new db field added
    statOpenSpots.innerText = session.totalSpots - session.filledSpots;
    statWaitlist.innerText = session.spotsInWaitingList;
}

function updateChart() {
    noChartElement.classList.add("d-none");
    canvas.style.display = "block";

    // TODO update when new db field added
    chart = upsert(canvas, {
        config: chartConfig,
        labels: ["Filled Spots", "Open Spots"],
        data: [session.filledSpots, session.openSpots]
    });
}

function renderAttendeeList() {
    if (loadingElement) loadingElement.remove();
    listContainer.innerHTML = '';

    session.attendees.forEach(attendee => {
        let card = attendeeTemplate.content.cloneNode(true);

        card.querySelector(".js-attendeeName").innerText = attendee.userName;
        const statusBadge = card.querySelector(".js-attendeeStatus");
        statusBadge.innerText = "To be determined";

        // TODO refactor
        // if (attendee.status === "Waitlist") {
        //     statusBadge.classList.replace("bg-secondary", "bg-warning");
        //     statusBadge.classList.add("text-dark");
        // } else {
        //     statusBadge.classList.replace("bg-secondary", "bg-success");
        // }

        listContainer.append(card);
    });
}

function showNoData() {
    noChartElement.classList.remove("d-none");
    canvas.style.display = "none";
    if (loadingElement) loadingElement.innerText = "Geen aanmeldingen gevonden.";
}