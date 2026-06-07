import { getConfig, upsert } from '../chartHelper.js';

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

const filterSelect = document.getElementById("filterSelect");
const sortSelect = document.getElementById("sortSelect");

const eventId = parseInt(document.querySelector('meta[name="event-id"]').content);
const sessionId = parseInt(document.querySelector('meta[name="session-id"]').content);
const chartConfig = getConfig({ cutout: "60%" });
let chart;

let session;

updatePage();
// setInterval(initPage, 5000);

async function updatePage() {
    await fetchSessionDetails();

    if (!session || session.totalSpots === 0) {
        showNoData();
        return;
    }

    updateStats();
    updateChart();
    renderAttendeeList();
    filterSelect.addEventListener("change", renderAttendeeList);
    sortSelect.addEventListener("change", renderAttendeeList);
}

async function fetchSessionDetails() {
    const response = await fetch(`/${eventId}/session/GetSpotsData/${sessionId}`);
    const data = await response.json();
    if (data && data.length > 0) {
        session = data[0];
    }
}

function isSessionStarted() {
    console.log('hi');
    console.log(session.startTime);
    if (!session || !session.startTime) return false;
    return new Date(session.startTime) <= new Date();
}

function getAttendanceCounts() {
    const signedUpAttendees = session.attendees ? session.attendees.filter(a => !a.inWaitingList) : [];
    const present = signedUpAttendees.filter(a => a.isAttending).length;
    const absent = signedUpAttendees.filter(a => !a.isAttending).length;
    return { present, absent };
}

function updateStats() {
    statTotalSpots.innerText = session.totalSpots;
    statWaitlist.innerText = session.spotsInWaitingList;
    statOpenSpots.innerText = session.totalSpots - session.filledSpots;

    if (isSessionStarted()) {
        const { present, absent } = getAttendanceCounts();
        statPresent.innerText = present;
        statAbsent.innerText = absent;
    } else {
        statPresent.innerText = 0;
        statAbsent.innerText = 0;
    }
}

function updateChart() {
    noChartElement.classList.add("d-none");
    canvas.style.display = "block";

    let labels, data;

    if (isSessionStarted()) {
        const { present, absent } = getAttendanceCounts();
        labels = ["Present", "Absent", "Open Spots", "In Waitlist"];
        data = [present, absent, session.totalSpots - session.filledSpots, session.spotsInWaitingList];
    } else {
        labels = ["Filled Spots", "Open Spots", "In Waitlist"];
        data = [session.filledSpots, session.totalSpots - session.filledSpots, session.spotsInWaitingList];
    }

    chart = upsert(canvas, {
        config: chartConfig,
        labels: labels,
        data: data
    });
}

function renderAttendeeList() {
    if (loadingElement) loadingElement.remove();
    listContainer.innerHTML = '';

    const filterValue = filterSelect.value;
    const sortValue = sortSelect.value;

    let attendees = [...session.attendees];

    if (filterValue === "signedup") {
        attendees = attendees.filter(a => !a.inWaitingList);
    } else if (filterValue === "waitlist") {
        attendees = attendees.filter(a => a.inWaitingList);
    }

    attendees.sort((a, b) => {
        switch (sortValue) {
            case "name-asc": return a.userName.localeCompare(b.userName);
            case "name-desc": return b.userName.localeCompare(a.userName);
            case "date-desc": return new Date(b.joinedDate) - new Date(a.joinedDate);
            case "date-asc": return new Date(a.joinedDate) - new Date(b.joinedDate);
            default: return 0;
        }
    });

    if (attendees.length === 0) {
        listContainer.innerHTML = '<div class="text-center text-muted mt-3">No attendees match this filter.</div>';
        return;
    }

    const hasStarted = isSessionStarted();

    attendees.forEach(attendee => {
        let card = attendeeTemplate.content.cloneNode(true);
        card.querySelector(".js-attendeeName").innerText = attendee.userName;
        const statusBadge = card.querySelector(".js-attendeeStatus");

        if (attendee.inWaitingList === true) {
            statusBadge.classList.replace("bg-secondary", "bg-warning");
            statusBadge.classList.add("text-dark");
            statusBadge.innerText = "In waiting list";
        } else {
            if (hasStarted) {
                if (attendee.isAttending) {
                    statusBadge.classList.replace("bg-secondary", "bg-success");
                    statusBadge.innerText = "Present";
                } else {
                    statusBadge.classList.replace("bg-secondary", "bg-danger");
                    statusBadge.innerText = "Absent";
                }
            } else {
                statusBadge.classList.replace("bg-secondary", "bg-success");
                statusBadge.innerText = "Signed up";
            }
        }
        listContainer.append(card);
    });
}

function showNoData() {
    noChartElement.classList.remove("d-none");
    canvas.style.display = "none";
    if (loadingElement) loadingElement.innerText = "Geen aanmeldingen gevonden.";
}