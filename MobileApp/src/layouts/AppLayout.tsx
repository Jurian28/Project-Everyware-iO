import { useEffect, useState, type ReactNode } from 'react';
import { View } from 'react-native';
import NotificationToast from '../components/NotificationToast';
import AuthService from '../services/AuthService';
import NotificationService, {
  Notification,
} from '../services/NotificationService';
import { appLayoutStyles } from '../styles/appLayoutStyles';

export default function AppLayout({
  children,
}: Readonly<{ children: ReactNode }>) {
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [notificationInterval, setNotificationInterval] =
    useState<NodeJS.Timeout | null>(null);

  async function checkNotifications() {
    if (await AuthService.isAuthenticated()) {
      const response = await NotificationService.getNotifications();

      if (response.success && response.notifications) {
        setNotifications(response.notifications);
      }
    }
  }

  useEffect(() => {
    if (!notificationInterval) {
      checkNotifications();
      setNotificationInterval(setInterval(checkNotifications, 30_000));
    }

    return () => {
      if (notificationInterval) {
        clearInterval(notificationInterval);
      }
    };
  }, [notificationInterval]);

  return (
    <View style={appLayoutStyles.container}>
      {notifications.length > 0 && (
        <View style={appLayoutStyles.notificationContainer}>
          {notifications.map(notification => (
            <NotificationToast
              key={notification.id}
              id={notification.id}
              title={notification.title}
              message={notification.content}
            />
          ))}
        </View>
      )}

      {children}
    </View>
  );
}
