import { View, Text, Pressable, Image } from 'react-native';
import { Event } from '../../services/EventService';
import { useRoute } from '@react-navigation/native';

export function EventPage() {
  const route = useRoute<any>();
  const { event } = route.params as { event: Event };

  return (
    <View style={{ padding: 12 }}>
      <View>
        <Pressable>
          <Image source={require('../../../public/icons/arrow-back.svg')} />
        </Pressable>
        {/* Back button */}
      </View>
      <View>
        <Text numberOfLines={2} style={{ fontSize: 24, fontWeight: 'bold', marginBottom: 12 }}>{event.title}</Text>

        <Image source={{ uri: "" }} style={{ width: '100%', height: 200, marginBottom: 12 }} />

        <View>
          <Text style={{ fontSize: 14, marginBottom: 8, fontWeight: 'bold', color: '#667' }}>
            {event.startDate.toLocaleDateString()} - {event.endDate.toLocaleDateString()}
          </Text>

          {event.location && (
            <Text style={{ fontSize: 14, marginBottom: 8, fontWeight: 'bold', color: '#667' }}>
              {event.location}
            </Text>
          )}

          <Pressable style={{ marginTop: 12, padding: 12, backgroundColor: event.mainColorHex, borderRadius: 8 }}>
            <Text style={{ color: '#fff', fontWeight: 'bold' }}>Enroll for Event (TODO)</Text>
          </Pressable>
        </View>

        {event.description && (
          <Text style={{ fontSize: 14, color: '#667' }}>{event.description}</Text>
        )}
      </View>
    </View>
  )
}