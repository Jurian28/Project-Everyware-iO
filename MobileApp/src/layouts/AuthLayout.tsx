import { type ReactNode } from 'react';
import { KeyboardAvoidingView, Platform, Text, View } from 'react-native';
import authStyles from '../styles/authStyles';

type AuthLayoutProps = {
  readonly title: string;
  readonly subtitle: string;
  readonly children: ReactNode;
  readonly watermarkText?: string;
};

export default function AuthLayout({
  title,
  subtitle,
  children,
  watermarkText,
}: Readonly<AuthLayoutProps>) {
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
      {!!watermarkText && (
        <Text style={authStyles.watermarkText}>{watermarkText}</Text>
      )}
    </KeyboardAvoidingView>
  );
}
