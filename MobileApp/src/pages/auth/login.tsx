import { useState } from 'react';
import { Alert, Button, Text, TextInput, View } from 'react-native';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  function submitForm() {
    if (!email.trim() || !password) {
      Alert.alert('Validation', 'Please fill in all fields.');
      return;
    }

    Alert.alert(
      'Login form submitted',
      `Email: ${email}\nPassword: ${password}`,
    );
  }

  return (
    <View>
      <Text>Email</Text>
      <TextInput
        autoCapitalize="none"
        autoComplete="email"
        keyboardType="email-address"
        placeholder="Email address"
        value={email}
        onChangeText={setEmail}
      />

      <Text>Password</Text>
      <TextInput
        autoCapitalize="none"
        autoComplete="password"
        keyboardType="visible-password"
        placeholder="Password"
        value={password}
        onChangeText={setPassword}
        secureTextEntry
      />

      <Button onPress={submitForm} title="Login" />
    </View>
  );
}
