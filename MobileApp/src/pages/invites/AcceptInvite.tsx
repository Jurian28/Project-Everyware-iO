import { useNavigation, type NavigationProp, type ParamListBase } from '@react-navigation/native';
import React, { useState } from 'react';
import {
  ActivityIndicator,
  Pressable,
  StyleSheet,
  Text,
  TextInput,
  View,
} from 'react-native';
import { InviteService } from '../../services/InviteService';

export default function AcceptInvitePage() {
  const navigation = useNavigation<NavigationProp<ParamListBase>>();
  const [inviteCode, setInviteCode] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const handleAccept = async () => {
    const token = inviteCode.trim().toUpperCase();
    if (!/^[A-Z0-9]{4}-[A-Z0-9]{4}$/.test(token)) {
      setError('Enter a valid invite code (format: XXXX-XXXX).');
      return;
    }

    setLoading(true);
    setError(null);

    const result = await InviteService.acceptInvite(token);

    setLoading(false);

    if (result.success) {
      setSuccess(true);
    } else {
      setError(result.message);
    }
  };

  if (success) {
    return (
      <View style={styles.container}>
        <Text style={styles.successTitle}>You joined the event!</Text>
        <Text style={styles.successText}>
          The event has been added to your account. Go to Events to find it.
        </Text>
        <Pressable style={styles.button} onPress={() => navigation.navigate('Events')}>
          <Text style={styles.buttonText}>Go to Events</Text>
        </Pressable>
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <Pressable style={styles.back} onPress={() => navigation.goBack()}>
        <Text style={styles.backText}>← Back</Text>
      </Pressable>

      <Text style={styles.title}>Accept Invite</Text>
      <Text style={styles.subtitle}>
        Enter the invite code from your invitation link to join an event.
      </Text>

      <TextInput
        style={styles.input}
        placeholder="XXXX-XXXX"
        autoCapitalize="characters"
        autoCorrect={false}
        value={inviteCode}
        onChangeText={v => {
          const clean = v.toUpperCase().replace(/[^A-Z0-9-]/g, '');
          const digits = clean.replace(/-/g, '');
          const formatted = digits.length > 4
            ? `${digits.slice(0, 4)}-${digits.slice(4, 8)}`
            : digits;
          setInviteCode(formatted);
          setError(null);
        }}
        maxLength={9}
        editable={!loading}
      />

      {error && <Text style={styles.error}>{error}</Text>}

      <Pressable
        style={[styles.button, loading && styles.buttonDisabled]}
        onPress={handleAccept}
        disabled={loading}
      >
        {loading
          ? <ActivityIndicator color="#fff" />
          : <Text style={styles.buttonText}>Accept Invite</Text>
        }
      </Pressable>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    padding: 24,
    backgroundColor: '#F8F9FA',
    justifyContent: 'center',
  },
  back: {
    position: 'absolute',
    top: 48,
    left: 24,
  },
  backText: {
    fontSize: 16,
    color: '#1E293B',
  },
  title: {
    fontSize: 26,
    fontWeight: '700',
    color: '#1E293B',
    marginBottom: 8,
  },
  subtitle: {
    fontSize: 14,
    color: '#64748B',
    marginBottom: 24,
    lineHeight: 20,
  },
  input: {
    backgroundColor: '#FFFFFF',
    borderWidth: 1,
    borderColor: '#CBD5E1',
    borderRadius: 8,
    paddingHorizontal: 16,
    paddingVertical: 12,
    fontSize: 16,
    color: '#1E293B',
    marginBottom: 12,
  },
  error: {
    color: '#EF4444',
    fontSize: 14,
    marginBottom: 12,
  },
  button: {
    backgroundColor: '#1E293B',
    paddingVertical: 14,
    borderRadius: 8,
    alignItems: 'center',
  },
  buttonDisabled: {
    opacity: 0.6,
  },
  buttonText: {
    color: '#FFFFFF',
    fontSize: 16,
    fontWeight: '600',
  },
  successTitle: {
    fontSize: 24,
    fontWeight: '700',
    color: '#16A34A',
    marginBottom: 12,
    textAlign: 'center',
  },
  successText: {
    fontSize: 14,
    color: '#64748B',
    textAlign: 'center',
    marginBottom: 32,
    lineHeight: 20,
  },
});
