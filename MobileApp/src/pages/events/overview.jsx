import { useEffect, useState } from 'react';
import { Alert, Button, Text, TextInput, View } from 'react-native';
import config from '../config';

export default function EventsOverview({ userId }) {
    const [events, setEvents] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(async () => {
        setLoading(true);

        // Fetch events from an API or database
        const response = await fetch(`${config.apiBaseUrl}/events/user/${userId}`);
        
        if(response.ok) {
            const data = await response.json();
            setEvents(data);
        }
        
        setLoading(false);
    }, []);

    if(loading) return <Text>Loading...</Text>;

    return (
        <View>
            {events.length === 0 ? (
                <Text>No events available.</Text>
            ) : (
                <div>
                    {events.map((event) => (
                        <View key={event.IdEvent}>
                            <Text>{event.Title}</Text>
                            <Text>{event.StartDate}</Text>
                            <Text>{event.EndDate}</Text>
                        </View>
                    ))}
                </div>
            )}
        </View>
    )
}