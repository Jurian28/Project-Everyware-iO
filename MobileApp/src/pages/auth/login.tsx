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

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const navigator = useNavigation<NavigationProp<ParamListBase>>();

  async function submitForm() {
    if (isSubmitting) {
      return;
    }

    if (!email.trim() || !password) {
      Alert.alert('Validation', 'Please fill in all fields.');
      return;
    }

    setIsSubmitting(true);

    try {
      const loggedIn = await AuthService.login(email.trim(), password);

      if (loggedIn) {
        navigator.navigate('Home');
      } else {
        Alert.alert('Login Failed', 'Invalid email or password.');
      }
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <AuthLayout title="Welcome back" subtitle="Sign in to continue">
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
        keyboardType="visible-password"
        placeholder="Password"
        placeholderTextColor="#7c8698"
        style={authStyles.input}
        value={password}
        onChangeText={setPassword}
        secureTextEntry
      />

      <Pressable
        onPress={submitForm}
        disabled={isSubmitting}
        style={({ pressed }) => [
          authStyles.button,
          pressed && authStyles.buttonPressed,
          isSubmitting && authStyles.buttonDisabled,
        ]}
      >
        <Text style={authStyles.buttonText}>
          {isSubmitting ? 'Signing in...' : 'Login'}
        </Text>
      </Pressable>

      <View style={authStyles.linkRow}>
        <Text style={authStyles.linkLabel}>No account yet?</Text>
        <Pressable onPress={() => navigator.navigate('Register')}>
          <Text style={authStyles.linkText}>Register</Text>
        </Pressable>
      </View>
    </AuthLayout>
  );
}
