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

initPage();

async function initPage() {
    const data = await fetchSessionDetails();

    if (!data || data.totalSpots === 0) {
        showNoData();
        return;
    }

    updateStats(data);
    updateChart(data);
    renderAttendeeList(data.attendees);
}

// --- 3. Data ophalen (met Dummy Data voor nu) ---
async function fetchSessionDetails() {
    const response = await fetch(`/${eventId}/session/GetSpotsData/${sessionId}`);
    const data = await response.json();
    if (data && data.length > 0) {
        session = data[0];
    }
    return ({
        totalSpots: session.totalSpots,
        present: 0, //TODO te implementeren
        absent: 0, //TODO te implementeren
        openSpots: session.totalSpots - session.filledSpots,
        waitlistCount: session.spotsInWaitingList,
        attendees: null //TODO add
    });
}

// --- 4. Weergave updaten ---
function updateStats(data) {
    statTotalSpots.innerText = data.totalSpots;
    statPresent.innerText = data.present;
    statAbsent.innerText = data.absent;
    statOpenSpots.innerText = data.openSpots;
    statWaitlist.innerText = data.waitlistCount;
}

function updateChart(data) {
    noChartElement.classList.add("d-none");
    canvas.style.display = "block";

    // Teken de grafiek met 2 waardes (zoals we hadden afgesproken)
    chart = upsert(canvas, {
        config: chartConfig,
        labels: ["Filled Spots", "Open Spots"],
        data: [data.filledSpots, data.openSpots]
    });
}

function renderAttendeeList(attendees) {
    if (loadingElement) loadingElement.remove();
    listContainer.innerHTML = ''; // Maak container leeg

    attendees.forEach(attendee => {
        let card = attendeeTemplate.content.cloneNode(true);

        card.querySelector(".js-attendeeName").innerText = attendee.name;
        const statusBadge = card.querySelector(".js-attendeeStatus");
        statusBadge.innerText = attendee.status;

        // Kleur de badge op basis van status (Bootstrap classes)
        if (attendee.status === "Waitlist") {
            statusBadge.classList.replace("bg-secondary", "bg-warning");
            statusBadge.classList.add("text-dark");
        } else {
            statusBadge.classList.replace("bg-secondary", "bg-success");
        }

        listContainer.append(card);
    });
}

function showNoData() {
    noChartElement.classList.remove("d-none");
    canvas.style.display = "none";
    if (loadingElement) loadingElement.innerText = "Geen aanmeldingen gevonden.";
}