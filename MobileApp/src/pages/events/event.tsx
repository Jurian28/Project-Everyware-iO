import { View, Text } from 'react-native';
import { Event } from '../../services/EventService';

export function EventPage({ event }: { event: Event }) {
  return (
    <View style={{ padding: 12 }}>
      <Text style={{ fontSize: 24, fontWeight: 'bold', marginBottom: 12 }}>{event.title}</Text>
      <Text style={{ fontSize: 14, marginBottom: 8, color: '#666' }}>
        {event.startDate.toLocaleDateString()} - {event.endDate.toLocaleDateString()}
      </Text>
      {event.description && (
        <Text style={{ fontSize: 14, color: '#666' }}>{event.description}</Text>
      )}
    </View>
  )
}