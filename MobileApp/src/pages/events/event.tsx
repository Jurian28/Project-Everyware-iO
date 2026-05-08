import { View, Text, Pressable, Image } from 'react-native';
import { Event, EventService } from '../../services/EventService';
import { useNavigation, useRoute } from '@react-navigation/native';
import { useEffect, useState } from 'react';
import config from '../../config';

export function EventPage() {
  const route = useRoute<any>();
  const { event } = route.params as { event: Event };

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
    <View style={{ padding: 12 }}>
      <View>
        <Pressable onPress={() => navigation.navigate('EventsOverview')} style={{ padding: 8, marginBottom: 4, width: '20%' }}>
          <svg xmlns="http://www.w3.org/2000/svg" height={28} width={28} viewBox="0 0 640 640">
            <path d="M73.4 297.4C60.9 309.9 60.9 330.2 73.4 342.7L233.4 502.7C245.9 515.2 266.2 515.2 278.7 502.7C291.2 490.2 291.2 469.9 278.7 457.4L173.3 352L544 352C561.7 352 576 337.7 576 320C576 302.3 561.7 288 544 288L173.3 288L278.7 182.6C291.2 170.1 291.2 149.8 278.7 137.3C266.2 124.8 245.9 124.8 233.4 137.3L73.4 297.3z"/>
          </svg>
        </Pressable>
        {/* Back button */}
      </View>
      <View style={{ display: 'flex', gap: 8 }}>
        <Text numberOfLines={2} style={{ fontSize: 24, fontWeight: 'bold', marginBottom: 12 }}>{event.title}</Text>

        <View style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', marginBottom: 4 }}>
          {imgSrc ? 
              <Image source={{ uri: file || "" }} style={{ width: 150, height: 150, marginBottom: 8 }} /> 
          : 
              <Text style={{ width: 150, height: 150, marginBottom: 8, backgroundColor: '#ccc', textAlign: 'center', lineHeight: 150 }}>No Image</Text>
          }
        </View>

        <View>
          <Text style={{ fontSize: 14, marginBottom: 8, fontWeight: 'bold', color: '#667' }}>
            {event.startDate.toLocaleDateString()} - {event.endDate.toLocaleDateString()}
          </Text>

          {event.location && (
            <Text style={{ fontSize: 14, marginBottom: 8, fontWeight: 'bold', color: '#667' }}>
              {event.location}
            </Text>
          )}
        </View>

        <View style={{ height: 1, backgroundColor: event.mainColorHex, marginVertical: 10 }} />

        {event.description && (
          <View style={{ display: 'flex', gap: 4 }} >
            <Text style={{ fontSize: 16, fontWeight: 'bold' }}>Description</Text>
            <Text style={{ fontSize: 14 }}>{event.description}</Text>
          </View>
        )}
      </View>
    </View>
  )
}