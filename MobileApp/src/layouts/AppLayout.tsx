import type { ReactNode } from 'react';
import { View } from 'react-native';

export default function AppLayout({
  children,
}: Readonly<{ children: ReactNode }>) {
  return <View>{children}</View>;
}
