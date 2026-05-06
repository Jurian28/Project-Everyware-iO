import { useEffect, useState } from 'react';
import { Alert, Pressable, Text, View, Image } from 'react-native';
import { EventService, Event } from '../../services/EventService';
import { useNavigation } from '@react-navigation/native';
import AppLayout from '../../layouts/AppLayout';

const eventsData: Event[] = [
    {
        idEvent: '1',
        title: 'What is Technology? Understanding the Systems That Shape Our Digital World',
        startDate: new Date(),
        endDate: new Date(),
        description: 'Understanding the Systems That Shape Our Digital World, IO brings together curious minds to explore how technology influences the way we live, work, and connect. ',
        location: 'Location 1',
        accentColorHex: '#3f3f3f',
        mainColorHex: '#ffa0a0',
        logoPath: null,
        isPublished: true,
    },
    {
        idEvent: '2',
        title: 'Event 2',
        startDate: new Date(),
        endDate: new Date(),
        description: 'Description for event 2',
        location: 'Location 2',
        accentColorHex: '#00FF00',
        mainColorHex: '#CCFFCC',
        logoPath: null,
        isPublished: false,
    }
];

export default function EventsOverview({ userId }: { userId: string }) {
    const [events, setEvents] = useState<Event[]>(eventsData);
    const [loading, setLoading] = useState(false);

    // useEffect(() => {
    //     const loadEvents = async () => {
    //         setLoading(true);

    //         const eventsResult = await EventService.fetchUserEvents(userId);
    //         if (!eventsResult.success) {
    //             Alert.alert('Error', 'Failed to load events. Please try again later.');
    //             setLoading(false);
    //             return;
    //         }

    //         setEvents(eventsResult.events || []);

    //         setLoading(false);
    //     };

    //     loadEvents();
    // }, [userId]);

    if (loading) return <Text>Loading...</Text>;

    return (
        <AppLayout>
            <View>
                {events.length === 0 ? (
                    <Text>No events available.</Text>
                ) : (
                    <View style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 12 }}>
                        <Text style={{ fontSize: 32, fontWeight: 'bold', marginBottom: 24, marginTop: 24 }}>My Events</Text>
                        <View style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 12, width: '100%' }}>
                            {events.map((event) => (
                                <EventCard key={event.idEvent} event={event} />
                            ))}
                        </View>
                    </View>
                )}
            </View>
        </AppLayout>
    )
}

function EventCard({ event }: { event: Event }) {
    const navigation = useNavigation();

    return (
        <Pressable onPress={() => console.log('Event card pressed')} style={{ padding: 12, marginBottom: 12, backgroundColor: event.mainColorHex, borderRadius: 8, width: '90%', maxHeight: 200 }}>
            <Text numberOfLines={2} style={{ fontSize: 18, fontWeight: 'bold', marginBottom: 8 }}>{event.title}</Text>
            
            <View style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', gap: 24 }}>
                <Image source={{ uri: 'https://fastly.picsum.photos/id/1/200/200.jpg?hmac=jZB9EZ0Vtzq-BZSmo7JKBBKJLW46nntxq79VMkCiBG8' }} style={{ width: 100, height: 100, marginBottom: 8 }} />
                <View style={{ display: 'flex', flexDirection: 'column', justifyContent: 'space-between', gap: 16, width: '65%' }}>
                    {event.description && (
                        <Text numberOfLines={3} style={{ marginBottom: 8 }}>{event.description}</Text>
                    )}
                    <Text style={{ fontSize: 12, marginBottom: 12 }}>
                        {event.startDate.toLocaleDateString()} - {event.endDate.toLocaleDateString()}
                    </Text>
                </View>
            </View>
            
        </Pressable>
    )
}