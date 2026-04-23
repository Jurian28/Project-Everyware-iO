import {
  type NavigationProp,
  type ParamListBase,
  useNavigation,
} from '@react-navigation/native';
import { useState } from 'react';
import { Alert, Pressable, Text, TextInput, View } from 'react-native';
import AuthLayout from '../../layouts/AuthLayout';
import AuthService from '../../services/AuthService';
import authStyles from '../../styles/authStyles';

export default function RegisterPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const navigation = useNavigation<NavigationProp<ParamListBase>>();

  const trimmedEmail = email.trim();
  const hasMissingFields = !trimmedEmail || !password || !confirmPassword;

  function goToLogin() {
    navigation.navigate('Login');
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

    if (hasMissingFields) {
      Alert.alert('Validation', 'Please fill in all fields.');
      return;
    }

    if (password !== confirmPassword) {
      Alert.alert('Validation', 'Passwords do not match.');
      return;
    }

    setIsSubmitting(true);

    try {
      const registerResult = await AuthService.register(trimmedEmail, password);

      if (registerResult.success) {
        navigation.navigate('Home');
      } else {
        Alert.alert(
          'Registration Failed',
          registerResult.message ??
            'An error occurred during registration. Please try again.',
        );
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <AuthLayout
      title="Create account"
      subtitle="Sign up to get started"
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
        secureTextEntry
        autoComplete="password-new"
        placeholder="Password"
        placeholderTextColor="#7c8698"
        style={authStyles.input}
        value={password}
        onChangeText={setPassword}
      />

      <Text style={authStyles.label}>Confirm password</Text>
      <TextInput
        secureTextEntry
        autoComplete="password-new"
        placeholder="Confirm password"
        placeholderTextColor="#7c8698"
        style={authStyles.input}
        value={confirmPassword}
        onChangeText={setConfirmPassword}
      />

      <Pressable
        onPress={handleSubmit}
        disabled={isSubmitting}
        style={getSubmitButtonStyle}
      >
        <Text style={authStyles.buttonText}>
          {isSubmitting ? 'Creating account...' : 'Register'}
        </Text>
      </Pressable>

      <View style={authStyles.linkRow}>
        <Text style={authStyles.linkLabel}>Already have an account?</Text>
        <Pressable onPress={goToLogin}>
          <Text style={authStyles.linkText}>Login</Text>
        </Pressable>
      </View>
    </AuthLayout>
  );
}
