import { useCallback, useEffect, useState } from 'react';
import { Pressable, Text, View } from 'react-native';
import NotificationService from '../services/NotificationService';

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
        <View style={styles.toastStyle}>
          <View style={styles.headerStyle}>
            <Text style={styles.titleStyle}>{title}</Text>
            <Pressable onPress={dismiss} style={styles.closeButtonStyle}>
              <Text style={styles.closeButtonTextStyle}>×</Text>
            </Pressable>
          </View>
          <Text style={styles.contentStyle}>{message}</Text>
        </View>
      )}
    </>
  );
}

const styles = {
  toastStyle: {
    backgroundColor: '#fff',
    borderRadius: 8,
    padding: 16,
    width: 320,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 5,
  },
  headerStyle: {
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  titleStyle: {
    fontSize: 18,
    fontWeight: '600',
    marginBottom: 8,
  },
  contentStyle: {
    fontSize: 14,
    color: '#4b5563',
  },
  closeButtonStyle: {
    padding: 4,
  },
};
