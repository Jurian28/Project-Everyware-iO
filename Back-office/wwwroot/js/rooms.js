const loadingMessage = document.getElementById("loadingMessage");
const noRoomsMessage = document.getElementById("noRoomsMessage");
const deleteButton = document.getElementById("deleteButton");
const searchParams = new URLSearchParams(window.location.search);
const scriptTag = document.currentScript;
const eventId = parseInt(scriptTag.dataset.eventId);


async function submitRoom(event) {

    event.preventDefault();

    if (!event.target.checkValidity()) {
        event.target.reportValidity();
        return;
    }

    const idRoom = document.getElementById("roomId").value;
    const room = {
        idroom: idRoom ? parseInt(idRoom) : null,
        roomLabel: document.getElementById("roomLabel").value,
        capacity: parseInt(document.getElementById("capacity").value),
        description: document.getElementById("description").value,
        idEvent: eventId,
    };

    const response = await fetch(`/${eventId}/room/data`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(room)
    });

    if (response.ok) {
        clearRoomForm()
        loadRooms();
    } else {
        const error = await response.text();
        console.error(error);
    }
}

async function deleteRoom() {
    const idRoom = document.getElementById("roomId").value;
    if (!idRoom) return;
    const response = await fetch(`/${eventId}/room/${idRoom}`, {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
    });
    if (response.ok)
    clearRoomForm();
    loadRooms();
}

function clearRoomForm() {
    document.getElementById("roomId").value = "";
    document.getElementById("roomLabel").value = "";
    document.getElementById("capacity").value = "";
    document.getElementById("description").value = "";

    deleteButton.classList.add("invisible");
}

function loadRoomForEdit(room) {
    document.getElementById("roomId").value = room.idRoom;
    document.getElementById("roomLabel").value = room.roomLabel;
    document.getElementById("capacity").value = room.capacity;
    document.getElementById("description").value = room.description ?? "";
    deleteButton.classList.remove("invisible");
}

async function loadRooms() {
    const response = await fetch(`/${eventId}/room/data`);
    const apiResponse = await response.json();
    const rooms = apiResponse.data;
    loadingMessage.classList.add("d-none");
    if (rooms) {
        renderRooms(rooms)
    }
    else {
        noRoomsMessage.classList.remove("d-none")
    }
}

function renderRooms(rooms) {
    const container = document.getElementById("roomList");
    container.innerHTML = "";
    if (!rooms) return;

    const template = document.getElementById("roomRowTemplate");
    container.innerHTML = "";

    rooms.forEach(room => {
        const clone = template.content.cloneNode(true);
        const row = clone.querySelector("div");

        clone.querySelector("[data-field='roomLabel']").textContent = room.roomLabel;
        clone.querySelector("[data-field='capacity']").textContent = room.capacity;
        clone.querySelector("[data-field='description']").textContent = room.description ?? "";

        row.addEventListener("click", () => loadRoomForEdit(room));
        container.appendChild(clone);
    });
}

loadRooms();