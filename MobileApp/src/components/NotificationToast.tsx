import { useCallback, useEffect, useState } from 'react';
import { Pressable, Text, View } from 'react-native';
import NotificationService from '../services/NotificationService';
import { toastStyles } from '../styles/notificationStyles';

export default function NotificationToast({
  id,
  title,
  message,
}: Readonly<{
  id: number;
  title: string;
  message: string;
}>) {
  const [shown, setShown] = useState(true);

  const dismiss = useCallback(() => {
    setShown(false);
    NotificationService.markAsRead(id);
  }, [id]);

  useEffect(() => {
    const timer = setTimeout(() => {
      dismiss();
    }, 30_000);

    return () => clearTimeout(timer);
  }, [dismiss, shown]);

  return (
    <>
      {shown && (
        <View style={toastStyles.toastStyle}>
          <View style={toastStyles.headerStyle}>
            <Text style={toastStyles.titleStyle}>{title}</Text>
            <Pressable onPress={dismiss} style={toastStyles.closeButtonStyle}>
              <Text style={toastStyles.closeButtonStyle}>×</Text>
            </Pressable>
          </View>
          <Text style={toastStyles.contentStyle}>{message}</Text>
        </View>
      )}
    </>
  );
}
