import { Notifications } from 'react-native-notifications';
import config from '../config';
import AuthService from './AuthService';

export type Notification = {
    id: string;
    title: string;
    content: string;
}

export default class NotificationService {
    private static readonly _baseUrl = `${config.apiBaseUrl}/notification`;
    private static readonly _notificationCategory = "default";

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

        return { success: true, notifications: json.data ?? [] };
    }

    public static async markNotificationsAsRead(notifications: Notification[]) {
        if (notifications.length === 0) {
            return;
        }

        const notificationIds = notifications.map(notification => notification.id);
        const response = await fetch(`${NotificationService._baseUrl}/mark-as-read`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ notificationIds }),
        });

        if (!response.ok) {
            console.error(`Failed to mark notifications as read: ${response.status} ${await response.text()}`);
            return { success: false, message: "Failed to mark notifications as read" }
        }
    }

    public static async showNotifications(notifications: Notification[]) {
        for (const notification of notifications) {
            Notifications.postLocalNotification({
                title: notification.title,
                body: notification.content,
                sound: "chime.aiff",
                silent: false,
                category: this._notificationCategory,
                fireDate: new Date(),
            });
        }

        await this.markNotificationsAsRead(notifications);
    }

    public static async checkAndShowNotifications() {
        const notifications = await this.getNotifications();

        if (notifications.success && notifications.notifications) {
            await this.showNotifications(notifications.notifications);
        }
    }
}

