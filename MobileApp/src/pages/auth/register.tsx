import { useState } from 'react';
import { Alert, Button, Text, TextInput, View } from 'react-native';

export default function RegisterScreen() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  function onSubmit() {
    if (!email.trim() || !password || !confirmPassword) {
      Alert.alert('Validation', 'Please fill in all fields.');
      return;
    }

    if (password !== confirmPassword) {
      Alert.alert('Validation', 'Passwords do not match.');
      return;
    }

    Alert.alert('Success', 'Registration form is valid.');
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
