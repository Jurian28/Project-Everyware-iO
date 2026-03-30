const eventId = 1;
const loadingMessage = document.getElementById("loadingMessage");
const noRoomsMessage = document.getElementById("noRoomsMessage");

async function submitRoom() {
    const roomId = document.getElementById("roomId").value;
    const room = {
        roomLabel: document.getElementById("roomLabel").value,
        capacity: parseInt(document.getElementById("capacity").value),
        description: document.getElementById("description").value,
        idEvent: parseInt(document.getElementById("eventId").value)
    };

    const isUpdate = roomId !== "";
    const url = isUpdate ? `/room/data/${roomId}` : "/room/data";
    const method = isUpdate ? "PUT" : "POST";

    const response = await fetch(url, {
        method: method,
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(room)
    });

    if (response.ok) {
        document.getElementById("roomId").value = "";
    } else {
        const error = await response.text();
        console.error(error);
    }
}

function loadRoomForEdit(room) {
    document.getElementById("roomId").value = room.idRoom;
    document.getElementById("roomLabel").value = room.roomLabel;
    document.getElementById("capacity").value = room.capacity;
    document.getElementById("description").value = room.description ?? "";
}

async function loadRooms() {
    const response = await fetch(`/room/data?eventId=${eventId}`);
    const rooms = await response.json();
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