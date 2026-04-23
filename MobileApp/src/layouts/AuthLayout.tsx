import { type ReactNode } from 'react';
import { KeyboardAvoidingView, Platform, Text, View } from 'react-native';
import authStyles from '../styles/authStyles';

type AuthLayoutProps = {
  title: string;
  subtitle: string;
  children: ReactNode;
};

export default function AuthLayout({
  title,
  subtitle,
  children,
}: AuthLayoutProps) {
  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === 'ios' ? 'padding' : undefined}
      style={authStyles.root}
    >
      <View style={authStyles.card}>
        <Text style={authStyles.title}>{title}</Text>
        <Text style={authStyles.subtitle}>{subtitle}</Text>
        {children}
      </View>
    </KeyboardAvoidingView>
  );
}
