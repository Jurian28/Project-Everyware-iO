import { getConfig, upsert, changeSingularDataSetData } from '../chartHelper.js'

const SESSION_CARD_PREFIX = "sessionCard";

const chartContainer = document.getElementById("chartContainer");
const canvas = document.createElement("canvas");
const chartConfig = getConfig({ cutout: "60%" });
const noChartElement = document.getElementById("noChart");

let filledSpots = 0;
let openSpots = 0;

let chart;

const eventId = parseInt(document.querySelector('meta[name="event-id"]').content);

const cardsContainer = document.getElementById("dashboardCardContainer");
const dashboardCardTemplate = document.getElementById("dashboardCardTemplate");

let percentages = [];

let sessions;

const noElementsElement = document.getElementById("noCards")
const loadingElement = document.getElementById("cardsLoading");
if (loadingElement) {
    loadingElement.classList.remove("d-none");
}
updatePage();
// setInterval(updatePage, 5000);

async function fetchSessions() {
    const response = await fetch(`/${eventId}/session/GetSpotsData`);
    const data = await response.json();
    if (data) {
        sessions = data;
        sessions = sessions.filter(s => !s.isPlenarySession);
    }
}
async function updatePage() {
    await fetchSessions();
    percentages = [];
    openSpots = 0;
    filledSpots = 0;
    updateOrCreatePageElements();
}

function updateOrCreatePageElements() {
    if (!sessions || sessions.length == 0 || (cardsContainer.childElementCount != sessions.length)) {
        createPageElements();
        return;
    }
    let create = false;
    for (const session of sessions) {
        const sessionCard = document.getElementById(`${SESSION_CARD_PREFIX}${session.sessionId}`)
        if (!sessionCard) {
            create = true;
            break;
        }
        setSessionCardData(sessionCard, session);

        cardsContainer.append(sessionCard);
    }
    if (create) {
        createPageElements();
        return;
    }
    setAverageAttendance();
    cardsContainer.classList.remove("d-none");
    updateOrCreateChart();
}
function createPageElements() {
    if (loadingElement) {
        loadingElement.classList.add("d-none");
    }
    if (!sessions || sessions.length == 0) {
        noElementsElement.classList.remove("d-none");
        noChartElement.classList.remove("d-none");
        return;
    }
    cardsContainer.innerHTML = '';
    sessions.forEach((session) => {
        let sessionCard = dashboardCardTemplate.content.cloneNode(true);
        sessionCard.firstElementChild.id = `${SESSION_CARD_PREFIX}${session.sessionId}`;
        setSessionCardData(sessionCard, session);

        cardsContainer.append(sessionCard);
    });
    setAverageAttendance();
    cardsContainer.classList.remove("d-none");
    updateOrCreateChart();
}

function setSessionCardData(sessionCard, session) {
    const titleElement = sessionCard.querySelector(".js-sessionTitle");
    const percentageElement = sessionCard.querySelector(".js-attendancePercentage");
    const detailsElement = sessionCard.querySelector(".js-detailsElement");

    openSpots += session.totalSpots - session.filledSpots;
    filledSpots += session.filledSpots;

    if (titleElement) {
        titleElement.innerText = session.sessionTitle
    }
    if (percentageElement) {
        let percentage = calculateAttendancepercentage(session);
        percentages.push(percentage);
        percentage = Math.round(percentage);
        percentageElement.innerText = `${percentage}%`
    }
    if (detailsElement) {
        detailsElement.setAttribute("href", `/${eventId}/DashBoard/${session.sessionId}`);
    }
}

function calculateAttendancepercentage(session) {

    return (session.filledSpots / session.totalSpots) * 100
}

function setAverageAttendance() {
    const attendanceSpan = document.getElementById("avgAttendance");
    if (attendanceSpan && percentages && percentages.length > 0) {
        attendanceSpan.parentElement.classList.remove("invisible");
        let percentageSum = 0;
        percentages.forEach((percentage) => {
            percentageSum += percentage;
        })
        const averageAttendance = Math.round(percentageSum / percentages.length);
        attendanceSpan.innerText = `${averageAttendance}%`
    }
}

function updateOrCreateChart() {
    if (chart) {
        changeSingularDataSetData(chart, null, [filledSpots, openSpots]);
        return;
    }

    chart = upsert(canvas, { config: chartConfig, labels: ["Filled", "Open"], data: [filledSpots, openSpots] });
    chartContainer.append(canvas);
}
