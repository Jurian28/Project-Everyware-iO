import config from '../config';
import AuthService from './AuthService';

export default class NotificationService {
    private static readonly _baseUrl = `${config.apiBaseUrl}/notification`;

    public static async getNotifications() {
        const userId = await AuthService.getUserId();
        const response = await fetch(`${NotificationService._baseUrl}/get-notifications-for-user/${userId}`);

        if (!response.ok) {
            console.error(`Failed to fetch notifications: ${response.status} ${await response.text()}`);
            return { success: false, message: "Failed to fetch notifications" }
        }

        const json = await response.json();

        if (!json.success) {
            console.error(`Failed to fetch notifications: ${response.status}`);
            return { success: false, message: "Failed to fetch notifications" }
        }

        return { success: true, notifications: json.data.notifications ?? [] };
    }
}

