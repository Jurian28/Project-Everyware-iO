import config from '../config';

export type TagResponseDTO = {
    idTag: number;
    idEvent: number;
    title: string;
    colorHex: string;
};

export type RoomResponseDTO = {
    idRoom: number;
    roomLabel: string;
    capacity: number;
};

export type SessionDTO = {
    sessionId: number;
    title: string;
    startTime: string;
    endTime: string;
    plenary: boolean;
    room: RoomResponseDTO;
    tags: TagResponseDTO[];
    speakerId?: number;
    speakerName: string;
};

type SessionsResult = {
    success: boolean;
    sessions?: SessionDTO[];
    message?: string;
};

export class SessionService {
    private static readonly _baseUrl = `${config.apiBaseUrl}`;

    public static async fetchEventSessions(eventId: string | number): Promise<SessionsResult> {
        try {
            const response = await fetch(`${config.apiBaseUrl}/${eventId}/sessions`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                },
            });

            if (!response.ok) {
                const message = await response.text();
                console.error(`Failed to fetch sessions for event ${eventId}:`, response.status, message);
                return { success: false, message: "Failed to fetch sessions" };
            }

            const json = await response.json();

            if (!json.success) {
                console.error(`API responded with success=false for event ${eventId}:`, json);
                return { success: false, message: "fetch returned unsuccessful" };
            }

            return { success: true, sessions: json.data };
        } catch (error) {
            console.error(`Fetching sessions for event ${eventId} ended with error:`, error);
            return { success: false, message: "Internal Server Error" };
        }
    }
}
