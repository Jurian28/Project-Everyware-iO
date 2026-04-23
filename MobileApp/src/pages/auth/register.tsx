import { useNavigation } from '@react-navigation/native';
import { useState } from 'react';
import { Alert, Button, Text, TextInput, View } from 'react-native';
import AuthService from '../../services/AuthService';

export default function RegisterScreen() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const navigator = useNavigation();

  async function onSubmit() {
    if (!email.trim() || !password || !confirmPassword) {
      Alert.alert('Validation', 'Please fill in all fields.');
      return;
    }

    if (password !== confirmPassword) {
      Alert.alert('Validation', 'Passwords do not match.');
      return;
    }

    const registered = await AuthService.register(email, password);

    if (registered) {
      navigator.navigate('Home');
    } else {
      Alert.alert(
        'Registration Failed',
        'An error occurred during registration. Please try again.',
      );
    }
  }

  return (
    <View>
      <Text>Register</Text>

      <TextInput
        autoCapitalize="none"
        autoComplete="email"
        keyboardType="email-address"
        placeholder="Email address"
        value={email}
        onChangeText={setEmail}
      />

      <TextInput
        secureTextEntry
        autoComplete="password-new"
        placeholder="Password"
        value={password}
        onChangeText={setPassword}
      />

      <TextInput
        secureTextEntry
        autoComplete="password-new"
        placeholder="Confirm password"
        value={confirmPassword}
        onChangeText={setConfirmPassword}
      />

      <Button onPress={onSubmit} title="Register" />
    </View>
  );
}
