import { useEffect, useState } from 'react';
import { Pressable, Text, View, Image } from 'react-native';
import { EventService, Event } from '../../services/EventService';
import { useNavigation } from '@react-navigation/native';
import config from '../../config';
import eventStyles from '../../styles/eventStyles';

export default function EventCard({ event }: { event: Event }) {
  const [file, setFile] = useState<string | null>(null);

  const navigation = useNavigation<any>();

  const imgSrc = event.logoPath ? `${config.apiBaseUrl}/${event.logoPath}` : null;

  useEffect(() => {
    async function fetchLogo() {
      try {
        const result = await EventService.getLogo(event.logoPath || '');

        if(!result.success || !result.file) {
          console.error('Failed to load event logo: API returned unsuccessful or no file', result);
          setFile(null);
          return;
        }

        const objectUrl = URL.createObjectURL(result.file || new Blob());

        setFile(objectUrl);
      } catch (error) {
        console.error('Failed to load event logo:', error);
        setFile(null);
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
            {event.startDate.toLocaleDateString()} - {event.endDate.toLocaleDateString()}
          </Text>
        </View>
      </View>
    </Pressable>
  )
}