import config from '../config';
import AuthService from './AuthService';

const baseUrl = config.apiBaseUrl;

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
  isEnrolled: boolean;
  room: RoomResponseDTO;
  tags: TagResponseDTO[];
  speakerId?: number;
  speakerName: string;
};

export type Tag = {
  idTag: number;
  title: string;
  colorHex: string;
};

export type Room = {
  roomLabel: string;
  capacity: number;
};

export type Session = {
  sessionId: number;
  title: string;
  startTime: string;
  endTime: string;
  plenary: boolean;
  room?: Room;
  tags?: Tag[];
  speakerName?: string;
  description?: string;
  placesLeft: number;
  isEnrolled?: boolean;
  inQueue?: boolean;
  queuePosition?: number;
};

type SessionsResult = {
  success: boolean;
  sessions?: SessionDTO[];
  message?: string;
};

async function safeJson(res: Response) {
  try {
    return await res.json();
  } catch {
    return null;
  }
}

export class SessionService {
  public static async fetchEventSessions(
    eventId: string | number,
  ): Promise<SessionsResult> {
    try {
      const response = await fetch(
        `${baseUrl}/${eventId}/sessions/withUserData`,
        {
          method: 'GET',
          headers: await AuthService.getAuthHeaders(),
        },
      );

      if (!response.ok) {
        const message = await response.text();
        console.error(
          `Failed to fetch sessions for event ${eventId}:`,
          response.status,
          message,
        );
        return { success: false, message: 'Failed to fetch sessions' };
      }

      const json = await response.json();

      if (!json.success) {
        console.error(
          `API responded with success=false for event ${eventId}:`,
          json,
        );
        return {
          success: false,
          message: 'fetch returned unsuccessful',
        };
      }

      return { success: true, sessions: json.data };
    } catch (error) {
      console.error(
        `Fetching sessions for event ${eventId} ended with error:`,
        error,
      );
      return { success: false, message: 'Internal Server Error' };
    }
  }

  public static async getSession(sessionId: number) {
    const res = await fetch(`${baseUrl}/sessions/${sessionId}`, {
      headers: await AuthService.getAuthHeaders(),
    });

    if (!res.ok) {
      return {
        success: false,
        message: 'Session not found',
      };
    }

    const json = await res.json();
    return json.data;
  }

  public static async enroll(
    sessionId: number,
    overrideConflict: boolean = false,
  ) {
    const res = await fetch(
      `${baseUrl}/sessions/${sessionId}/enroll?overrideSessions=${overrideConflict}`,
      {
        method: 'POST',
        headers: await AuthService.getAuthHeaders(),
      },
    );

    const data = await safeJson(res);

    if (data == null) {
      return {
        success: false,
        message: 'Enroll failed',
      };
    }

    const isConflict = data.data.conflictingSessions != null;

    return {
      success: data.success ?? false,
      message:
        data.message ?? (isConflict ? 'Enrollment conflict' : 'Enroll failed'),
      isConflict: isConflict,
      conflictingSessions: data.data.conflictingSessions,
    };
  }

  public static async withdraw(sessionId: number) {
    const res = await fetch(`${baseUrl}/sessions/${sessionId}/enroll`, {
      method: 'DELETE',
      headers: await AuthService.getAuthHeaders(),
    });

    const data = await safeJson(res);

    if (!res.ok) {
      return {
        success: false,
        message: data?.message ?? 'Withdraw failed',
      };
    }

    return { success: true };
  }

  public static async markAttendance(sessionId: number, userId: string) {
    const res = await fetch(`${baseUrl}/sessions/${sessionId}/attendance`, {
      method: 'POST',
      headers: await AuthService.getAuthHeaders(),
      body: JSON.stringify(userId),
    });

    const data = await safeJson(res);

    if (!res.ok) {
      return {
        success: false,
        message: data?.error ?? data?.message ?? 'Failed to mark attendance',
      };
    }

    return { success: true };
  }
}
