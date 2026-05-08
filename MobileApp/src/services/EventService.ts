import config from '../config';

export type Event = {
    idEvent: string;
    title: string;
    startDate: Date;
    endDate: Date;
    description: string | null;
    location: string;
    accentColorHex: string;
    mainColorHex: string;
    logoPath: string | null;
    isPublished: boolean;
}

type EventList = {
  success: boolean;
  data: {
    totalPages: number;
    events: Event[];
  }
}

type EventsResult = {
  success: boolean;
  events?: Event[];
  message?: string;
  file?: Blob;
};

export class EventService {
    private static readonly _baseUrl = `${config.apiBaseUrl}/event`;

    public static async fetchUserEvents(userId: string): Promise<EventsResult> {
      try {
        const response = await fetch(`${EventService._baseUrl}/user/${userId}`, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
          },
        });

        if(!response.ok) {
          const message = await response.text();
          console.error(`Failed to fetch events for user ${userId}:`, response.status, message, `${EventService._baseUrl}/user/${userId}`);
          return { success: false, message: "Failed to fetch events" };
        }

        const json: EventList = await response.json();

        if(!json.success) {
          console.error(`API responded with success=false for user ${userId}:`, json);
          return { success: false, message: "fetch returned unsuccessful" };
        }
        
        // Convert date strings to Date objects
        const events = json.data.events.map(event => ({
          ...event,
          startDate: new Date(event.startDate),
          endDate: new Date(event.endDate),
        }));
        
        return { success: true, events };
      } catch (error) {
        console.error(`Fetching events for user ${userId} ended with error:`, error);
        return { success: false, message: "Internal Server Error" };
      }
    }

    public static async getLogo(logoPath: string): Promise<EventsResult> {
      try {
        const response = await fetch(`${EventService._baseUrl}${logoPath}`, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
          },
        });

        if(!response.ok) {
          const message = await response.text();
          console.error(`Failed to fetch logo for path ${logoPath}:`, response.status, message, `${EventService._baseUrl}/${logoPath}`);
          return { success: false, message: "Failed to fetch logo" };
        }

        const file = await response.blob();
        
        return { success: true, file };
      } catch (error) {
        console.error(`Fetching logo for path ${logoPath} ended with error:`, error);
        return { success: false, message: "Internal Server Error" };
      }
    }
}