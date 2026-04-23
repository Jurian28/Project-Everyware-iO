import { useNavigation } from '@react-navigation/native';
import { useState } from 'react';
import { Alert, Button, Text, TextInput, View } from 'react-native';
import AuthService from '../../services/AuthService';

export default function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const navigator = useNavigation();

  async function submitForm() {
    if (!email.trim() || !password) {
      Alert.alert('Validation', 'Please fill in all fields.');
      return;
    }

    const loggedIn = await AuthService.login(email, password);

    if (loggedIn) {
      navigator.navigate('Home');
    } else {
      Alert.alert('Login Failed', 'Invalid email or password.');
    }
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
