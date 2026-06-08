import {
  type NavigationProp,
  type ParamListBase,
  useNavigation,
} from '@react-navigation/native';
import React, { useState } from 'react';
import { Alert, Pressable, Text, TextInput, View } from 'react-native';
import AuthLayout from '../../layouts/AuthLayout';
import AuthService from '../../services/AuthService';
import authStyles from '../../styles/authStyles';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const navigation = useNavigation<NavigationProp<ParamListBase>>();

  const trimmedEmail = email.trim();
  const isFormInvalid = !trimmedEmail || !password;

  function goToRegister() {
    navigation.navigate('Register');
  }

  function getSubmitButtonStyle({ pressed }: { pressed: boolean }) {
    return [
      authStyles.button,
      pressed && authStyles.buttonPressed,
      isSubmitting && authStyles.buttonDisabled,
    ];
  }

  async function handleSubmit() {
    if (isSubmitting) {
      return;
    }

    if (isFormInvalid) {
      Alert.alert('Validation', 'Please fill in all fields.');
      return;
    }

    setIsSubmitting(true);

    try {
      const loginResult = await AuthService.login(trimmedEmail, password);

      if (loginResult.success) {
        navigation.navigate('Home');
      } else {
        Alert.alert(
          'Login Failed',
          loginResult.message ?? 'Invalid email or password.',
        );
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <AuthLayout
      title="Welcome back"
      subtitle="Sign in to continue"
      watermarkText="Event Connect by iO"
    >
      <Text style={authStyles.label}>Email</Text>
      <TextInput
        autoCapitalize="none"
        autoComplete="email"
        keyboardType="email-address"
        placeholder="you@example.com"
        placeholderTextColor="#7c8698"
        style={authStyles.input}
        value={email}
        onChangeText={setEmail}
      />

      <Text style={authStyles.label}>Password</Text>
      <TextInput
        autoCapitalize="none"
        autoComplete="password"
        placeholder="Password"
        placeholderTextColor="#7c8698"
        style={authStyles.input}
        value={password}
        onChangeText={setPassword}
        secureTextEntry
      />

      <Pressable
        onPress={handleSubmit}
        disabled={isSubmitting}
        style={getSubmitButtonStyle}
      >
        <Text style={authStyles.buttonText}>
          {isSubmitting ? 'Signing in...' : 'Login'}
        </Text>
      </Pressable>

      <View style={authStyles.linkRow}>
        <Text style={authStyles.linkLabel}>No account yet?</Text>
        <Pressable onPress={goToRegister}>
          <Text style={authStyles.linkText}>Register</Text>
        </Pressable>
      </View>
    </AuthLayout>
  );
}
