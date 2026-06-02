import config from '../config';
import AuthService from './AuthService';

export type Notification = {
  id: string;
  title: string;
  content: string;
};

export type NotificationApiResponse = {
  success: boolean;
  notifications?: Notification[];
  message?: string;
};

export default class NotificationService {
  private static readonly _baseUrl = `${config.apiBaseUrl}/notification`;

  public static async getNotifications(): Promise<NotificationApiResponse> {
    const response = await fetch(
      `${NotificationService._baseUrl}/get-notifications`,
      {
        headers: await AuthService.getAuthHeaders(),
      },
    );

    if (!response.ok) {
      console.error(
        `Failed to fetch notifications: ${
          response.status
        } ${await response.text()}`,
      );
      return { success: false, message: 'Failed to fetch notifications' };
    }

    const json = await response.json();

    if (!json.success) {
      console.error(`Failed to fetch notifications: ${response.status}`);
      return { success: false, message: 'Failed to fetch notifications' };
    }

    return { success: true, notifications: json.data ?? [] };
  }

  public static async markAsRead(notificationId: number) {
    const response = await fetch(
      `${NotificationService._baseUrl}/mark-as-read/${notificationId}`,
      {
        method: 'POST',
        headers: await AuthService.getAuthHeaders(),
      },
    );

    if (!response.ok) {
      console.error(
        `Failed to mark notification ${notificationId} as read: ${
          response.status
        } ${await response.text()}`,
      );
    }
  }
}
