const loadingMessage = document.getElementById("loadingMessage");
const noRoomsMessage = document.getElementById("noRoomsMessage");
const deleteButton = document.getElementById("deleteButton");
const searchParams = new URLSearchParams(window.location.search);
const scriptTag = document.currentScript;
const eventId = parseInt(scriptTag.dataset.eventId);
const editButtons = document.getElementsByClassName("js-room-edit-button");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/rooms")
    .withAutomaticReconnect()
    .build();

connection.on("RoomAdded", (room) => {
    console.log("New room received:", room);
});

connection.start().catch(err => console.error("SignalR connection error:", err));


let idRoom = null;
$(document).keydown(function (e) {
    if (e.keyCode == 27) {
        clearRoomForm();
    }
});

async function submitRoom(event) {

    event.preventDefault();

    if (!event.target.checkValidity()) {
        event.target.reportValidity();
        return;
    }

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
        try {
            const isEditing = !!idRoom;
            document.getElementById("successMessageBox").innerText = `Successfully ${isEditing ? 'updated' : 'created'} a room with label: ${room.roomLabel}`;
            setTimeout(() => {
                document.getElementById("successMessageBox").innerText = "";
            }, 5000);
        } catch (e) {
            console.error(e)
        }
        clearRoomForm();
        loadRooms();
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

async function deleteRoom() {
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
    unselectRoomForEdit();
    idRoom = null;
    document.getElementById("roomId").value = "";
    document.getElementById("roomLabel").value = "";
    document.getElementById("capacity").value = "";
    document.getElementById("description").value = "";

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

function loadRoomForEdit(room) {
    unselectRoomForEdit();
    idRoom = room.idRoom;
    const roomRow = document.getElementById(`room-${idRoom}`);
    if (roomRow) {
        roomRow.classList.add("bg-selected");
        roomRow.classList.remove("bg-white");
    }

    document.getElementById("roomLabel").value = room.roomLabel;
    document.getElementById("capacity").value = room.capacity;
    document.getElementById("description").value = room.description ?? "";
    deleteButton.classList.remove("invisible");
    showEditButtons();
}

function unselectRoomForEdit() {
    if (!idRoom) return;
    const roomRow = document.getElementById(`room-${idRoom}`);
    roomRow.classList.remove("bg-selected");
    roomRow.classList.add("bg-white");
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
        row.id = `room-${room.idRoom}`;
        row.classList.add("bg-white");

        clone.querySelector("[data-field='roomLabel']").textContent = room.roomLabel;
        clone.querySelector("[data-field='capacity']").textContent = room.capacity;
        clone.querySelector("[data-field='description']").textContent = room.description ?? "";

        row.addEventListener("click", () => loadRoomForEdit(room));
        container.appendChild(clone);
    });
}

loadRooms();