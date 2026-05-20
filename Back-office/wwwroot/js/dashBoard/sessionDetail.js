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

const sessionId = parseInt(document.querySelector('meta[name="session-id"]').content);
const chartConfig = getConfig({ cutout: "60%" });
let chart;

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
    // TODO: Zodra je backend klaar is, uncomment de fetch-code:
    // const response = await fetch(`/Dashboard/GetSessionDetails?sessionId=${sessionId}`);
    // return await response.json();

    const response = await fetch(`/${eventId}/session/GetSpotsData/${sessionId}`);
    const data = await response.json();
    if (data) {
        sessions = data;
    }

    // TODO gebruik SessionSpotsDTO, en zet de data goed

    // DUMMY DATA (Simuleert een korte laadtijd van de server)
    return new Promise(resolve => setTimeout(() => {
        resolve({
            present: 3,
            absent: 2,
            openSpots: 5,
            totalSpots: 50,
            waitlistCount: 2,
            filledSpots: 45, // TotalSpots - OpenSpots
            attendees: [
                { name: "Jan Jansen", status: "Registered" },
                { name: "Piet Pietersen", status: "Registered" },
                { name: "Klaas Visser", status: "Waitlist" }
            ]
        });
    }, 500));
}

// --- 4. Weergave updaten ---
function updateStats(data) {
    statPresent.innerText = data.present;
    statAbsent.innerText = data.absent;
    statOpenSpots.innerText = data.openSpots;
    statTotalSpots.innerText = data.totalSpots;
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