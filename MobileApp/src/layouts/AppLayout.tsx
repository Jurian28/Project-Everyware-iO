import type { ReactNode } from 'react';
import { View } from 'react-native';

const styles = {
  container: {
    flex: 1,
  }
};

export default function AppLayout({
  children,
}: Readonly<{ children: ReactNode }>) {
  return <View style={styles.container}>{children}</View>;
}
