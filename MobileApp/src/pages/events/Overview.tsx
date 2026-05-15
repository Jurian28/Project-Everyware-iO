import { useEffect, useState } from 'react';
import { Alert, Text, View, Image, RefreshControl, ScrollView, Pressable } from 'react-native';
import { EventService, Event } from '../../services/EventService';
import AppLayout from '../../layouts/AppLayout';
import AuthService from '../../services/AuthService';
import eventStyles from '../../styles/eventStyles';
import EventCard from '../../components/event/EventCard';
import Colors from '../../pages/enums/colors';

export default function EventsOverview() {
  const [events, setEvents] = useState<Event[]>([]);
  const [loading, setLoading] = useState(false);
  const [includePastEvents, setIncludePastEvents] = useState(false);

  const loadEvents = async (includePastEvents: boolean = false) => {
    setLoading(true);

    try {
      const id = await AuthService.getUserId(); 
      
      if (id) {
        const eventsResult = await EventService.fetchUserEvents(id, includePastEvents);
    
        if (!eventsResult.success) {
          Alert.alert('Error', 'Failed to load events. Please try again later.');
          setLoading(false);
          return;
        }
    
        setEvents(eventsResult.events || []);
      }
    } catch (error) {
      Alert.alert('Error', 'Failed to load events. Please try again later.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadEvents();
  }, []);

  function onPress() {
    const newIncludePastEvents = !includePastEvents;
    setIncludePastEvents(newIncludePastEvents);
    
    loadEvents(newIncludePastEvents);
  }

  return (
    <AppLayout>
      <ScrollView 
        style={eventStyles.scrollViewContainer}
        showsVerticalScrollIndicator={false}
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

            <Pressable 
              onPress={onPress}
              style={[eventStyles.eventsLoadPastEvents, { backgroundColor: includePastEvents ? 'rgb(170, 170, 170)' : Colors.DEFAULT_BUTTON_COLOR }]}  
            >
              <Text style={eventStyles.eventsLoadPastEventsText}>
                {includePastEvents ? 'Hide Past Events' : 'Load Past Events'}
              </Text>
            </Pressable>

            <View style={eventStyles.eventsListContainer}>
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