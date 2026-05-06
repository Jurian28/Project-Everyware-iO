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
  totalPages: number;
  events: Event[];
}

type EventsResult = {
  success: boolean;
  events?: Event[];
};

export class EventService {
    private static readonly _baseUrl = `${config.apiBaseUrl}/events`;

    public static async fetchUserEvents(userId: string): Promise<EventsResult> {
      try {
        const response = await fetch(`${EventService._baseUrl}/${userId}`, {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
          },
        });

        if(!response.ok) {
          const message = await response.text();
          console.error(`Failed to fetch events for user ${userId}:`, response.status, message);
          return { success: false };
        }

        const json = await response.json();
        return { success: true, events: json.events };
      } catch (error) {
        console.error(`Failed to fetch events for user ${userId}:`, error);
        return { success: false };
      }
    }

    public static async togglePublish(eventId: string, published: boolean) {
        const url = `events/${eventId}/` + (published ? "unpublish" : "publish");
        try {
            const response = await fetch(url, {
                method: "POST",
                headers: {
                    'Content-Type': 'application/json'
                },
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            
        } catch (error) {
            console.error('Error:', error);
            throw error;
        }
    }
}