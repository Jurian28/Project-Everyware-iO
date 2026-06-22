import { useNavigation, type NavigationProp, type ParamListBase } from '@react-navigation/native';
import { Pressable, StyleSheet, Text, View } from 'react-native';
import AppLayout from '../layouts/AppLayout';

export default function HomePage() {
  const navigation = useNavigation<NavigationProp<ParamListBase>>();

  return (
    <AppLayout>
      <View style={styles.container}>
        <Text style={styles.welcome}>Welcome to iO Event Connect</Text>

        <Pressable
          style={styles.eventsButton}
          onPress={() => navigation.navigate('Events')}
        >
          <Text style={styles.eventsButtonText}>View Events</Text>
        </Pressable>

        <Pressable
          style={styles.inviteButton}
          onPress={() => navigation.navigate('AcceptInvite')}
        >
          <Text style={styles.inviteButtonText}>Accept Invite</Text>
        </Pressable>
      </View>
    </AppLayout>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    padding: 24,
    gap: 24,
  },
  welcome: {
    fontSize: 22,
    fontWeight: '600',
    color: '#1E293B',
    textAlign: 'center',
  },
  eventsButton: {
    backgroundColor: '#1E293B',
    paddingVertical: 14,
    paddingHorizontal: 32,
    borderRadius: 8,
    width: '100%',
    alignItems: 'center',
  },
  eventsButtonText: {
    color: '#FFFFFF',
    fontSize: 16,
    fontWeight: '600',
  },
  inviteButton: {
    borderWidth: 1.5,
    borderColor: '#1E293B',
    paddingVertical: 13,
    paddingHorizontal: 32,
    borderRadius: 8,
    width: '100%',
    alignItems: 'center',
  },
  inviteButtonText: {
    color: '#1E293B',
    fontSize: 16,
    fontWeight: '600',
  },
});
