import { View, Text, Pressable, Image } from 'react-native';
import { Event } from '../../services/EventService';
import { useNavigation, useRoute } from '@react-navigation/native';
import config from '../../config';
import eventStyles from '../../styles/eventStyles';

export function EventPage() {
  const route = useRoute<any>();
  const { event } = route.params as { event: Event };
  
  const navigation = useNavigation<any>();

  const imgSrc = event.logoPath ? `${config.apiBaseUrl}/event${event.logoPath}` : null;

  return (
    <View style={eventStyles.eventPageContainer}>
      <View>
        <Pressable onPress={() => navigation.navigate('EventsOverview')} style={eventStyles.backButton}>
          <Text style={eventStyles.backButton}>&larr;</Text>
        </Pressable>
      </View>
      <View style={eventStyles.eventContentContainer}>
        <Text numberOfLines={2} style={eventStyles.eventPageTitle}>{event.title}</Text>

        <View style={eventStyles.eventImageContainer}>
          {imgSrc ? 
              <Image 
                source={{ uri: imgSrc }} 
                style={eventStyles.eventPageImage}
              /> 
          : 
              <Text style={eventStyles.eventPageNoImage}>No Logo</Text>
          }
        </View>

        <View>
          <Text style={eventStyles.eventPageMeta}>
            {event.startDate.toLocaleDateString()} - {event.endDate.toLocaleDateString()}
          </Text>

          {event.location && (
            <Text style={eventStyles.eventPageMeta}>
              {event.location}
            </Text>
          )}
        </View>

        <View style={[eventStyles.eventPageDivider, { backgroundColor: event.mainColorHex }]} />

        <Pressable onPress={() => navigation.navigate('EventSchedule', { eventId: event.idEvent, eventTitle: event.title, eventColor: event.mainColorHex, eventAccentColor: event.accentColorHex })} style={{ backgroundColor: '#2563EB', padding: 12, borderRadius: 8, alignItems: 'center', marginVertical: 10 }}>
          <Text style={{ color: 'white', fontWeight: 'bold' }}>View Schedule</Text>
        </Pressable>

        {event.description && (
          <View style={eventStyles.eventPageDescriptionSection}>
            <Text style={eventStyles.eventPageDescriptionTitle}>Description</Text>
            <Text style={eventStyles.eventPageDescriptionText}>{event.description}</Text>
          </View>
        )}
      </View>
    </View>
  )
}