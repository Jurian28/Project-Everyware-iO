import { useEffect, useState } from 'react';
import { Alert, Pressable, Text, View, Image, RefreshControl, ScrollView } from 'react-native';
import { EventService, Event } from '../../services/EventService';
import { useNavigation } from '@react-navigation/native';
import AppLayout from '../../layouts/AppLayout';
import AuthService from '../../services/AuthService';
import config from '../../config';
import eventStyles from '../../styles/eventStyles';

const formatDate = (date: Date | string): string => {
  try {
    const d = typeof date === 'string' ? new Date(date) : date;
    return d.toLocaleDateString();
  } catch {
    return 'Invalid date';
  }
};

export default function EventsOverview() {
  const [events, setEvents] = useState<Event[]>([]);
  const [loading, setLoading] = useState(false);

  const loadEvents = async () => {
    setLoading(true);
    const id = await AuthService.getUserId(); 
        
    if (id) {
      const eventsResult = await EventService.fetchUserEvents(id);
      if (!eventsResult.success) {
        Alert.alert('Error', 'Failed to load events. Please try again later.');
        setLoading(false);
        return;
      }
      setEvents(eventsResult.events || []);
    }
    setLoading(false);
  };

  useEffect(() => {
    loadEvents();
  }, []);

  return (
    <AppLayout>
      <ScrollView
        style={eventStyles.scrollViewContainer}
        refreshControl={
          <RefreshControl refreshing={loading} onRefresh={loadEvents} />
        }
      >
        {events.length === 0 ? (
          <View style={eventStyles.eventsNoEventsContainer}>
            <Text style={eventStyles.eventsNoEvents}>No events found</Text>
          </View>
        ) : (
          <View style={eventStyles.eventsContainer}>
            <Text style={eventStyles.eventsTitle}>My Events</Text>
            <View style={eventStyles.eventsListContainer}>
              {events.map((event) => (
                <EventCard key={event.idEvent} event={event} />
              ))}
              {events.map((event) => (
                <EventCard key={event.idEvent} event={event} />
              ))}
            </View>
          </View>
        )}
      </ScrollView>
    </AppLayout>
  )
}

function EventCard({ event }: { event: Event }) {
  const [file, setFile] = useState<string | null>(null);

  const navigation = useNavigation<any>();

  const imgSrc = event.logoPath ? `${config.apiBaseUrl}/${event.logoPath}` : null;

  useEffect(() => {
    async function fetchLogo() {
      if (event.logoPath) {
        const result = await EventService.getLogo(event.logoPath || '');

        const objectUrl = URL.createObjectURL(result.file || new Blob());

        setFile(objectUrl);
      }
    }
    
    fetchLogo();
  }, [event.logoPath]);

  return (
    <Pressable onPress={() => navigation.navigate('Event', { event })} style={[eventStyles.eventCard, { backgroundColor: event.mainColorHex }]}>
            <Text numberOfLines={2} style={eventStyles.eventCardTitle}>{event.title}</Text>
            
            <View style={eventStyles.eventCardContentRow}>
                {imgSrc ? 
                    <Image source={{ uri: file || "" }} style={eventStyles.eventCardImage} /> 
                : 
                    <Text style={eventStyles.eventCardNoImage}>No Image</Text>
                }
                <View style={eventStyles.eventCardTextSection}>
                    {event.description && (
                        <Text numberOfLines={3} style={eventStyles.eventCardDescription}>{event.description}</Text>
                    )}
                    <Text style={eventStyles.eventCardDate}>
                        {formatDate(event.startDate)} - {formatDate(event.endDate)}
                    </Text>
                </View>
            </View>
    </Pressable>
  )
}