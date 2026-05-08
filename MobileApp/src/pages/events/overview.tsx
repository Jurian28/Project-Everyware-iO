import { use, useEffect, useState } from 'react';
import { Alert, Pressable, Text, View, Image, RefreshControl, ScrollView } from 'react-native';
import { EventService, Event } from '../../services/EventService';
import { useNavigation } from '@react-navigation/native';
import AppLayout from '../../layouts/AppLayout';
import AuthService from '../../services/AuthService';
import config from '../../config';

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
                refreshControl={
                    <RefreshControl refreshing={loading} onRefresh={loadEvents} />
                }
            >
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
        <Pressable onPress={() => navigation.navigate('Event', { event })} style={{ padding: 12, marginBottom: 12, backgroundColor: event.mainColorHex, borderRadius: 8, width: '90%', maxHeight: 200 }}>
            <Text numberOfLines={2} style={{ fontSize: 18, fontWeight: 'bold', marginBottom: 8 }}>{event.title}</Text>
            
            <View style={{ display: 'flex', flexDirection: 'row', alignItems: 'center', gap: 24 }}>
                {imgSrc ? 
                    <Image source={{ uri: file || "" }} style={{ width: 100, height: 100, marginBottom: 8 }} /> 
                : 
                    <Text style={{ width: 100, height: 100, marginBottom: 8, backgroundColor: '#ccc', textAlign: 'center', lineHeight: 100 }}>No Image</Text>
                }
                <View style={{ display: 'flex', flexDirection: 'column', justifyContent: 'space-between', gap: 16, width: '65%' }}>
                    {event.description && (
                        <Text numberOfLines={3} style={{ marginBottom: 8 }}>{event.description}</Text>
                    )}
                    <Text style={{ fontSize: 12, marginBottom: 12 }}>
                        {formatDate(event.startDate)} - {formatDate(event.endDate)}
                    </Text>
                </View>
            </View>
            
        </Pressable>
    )
}