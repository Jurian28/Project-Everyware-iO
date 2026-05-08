import { View, Text, Pressable, Image } from 'react-native';
import { Event, EventService } from '../../services/EventService';
import { useNavigation, useRoute } from '@react-navigation/native';
import { useEffect, useState } from 'react';
import config from '../../config';
import eventStyles from '../../styles/eventStyles';

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
    <View style={eventStyles.eventPageContainer}>
      <View>
        <Pressable onPress={() => navigation.navigate('EventsOverview')} style={eventStyles.backButton}>
          <svg xmlns="http://www.w3.org/2000/svg" height={28} width={28} viewBox="0 0 640 640">
            <path d="M73.4 297.4C60.9 309.9 60.9 330.2 73.4 342.7L233.4 502.7C245.9 515.2 266.2 515.2 278.7 502.7C291.2 490.2 291.2 469.9 278.7 457.4L173.3 352L544 352C561.7 352 576 337.7 576 320C576 302.3 561.7 288 544 288L173.3 288L278.7 182.6C291.2 170.1 291.2 149.8 278.7 137.3C266.2 124.8 245.9 124.8 233.4 137.3L73.4 297.3z"/>
          </svg>
        </Pressable>
        {/* Back button */}
      </View>
      <View style={eventStyles.eventContentContainer}>
        <Text numberOfLines={2} style={eventStyles.eventPageTitle}>{event.title}</Text>

        <View style={eventStyles.eventImageContainer}>
          {imgSrc ? 
              <Image source={{ uri: file || "" }} style={eventStyles.eventPageImage} /> 
          : 
              <Text style={eventStyles.eventPageNoImage}>No Image</Text>
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