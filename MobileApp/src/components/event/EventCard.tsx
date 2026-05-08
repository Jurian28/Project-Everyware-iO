import { Pressable, Text, View, Image } from 'react-native';
import { Event } from '../../services/EventService';
import { useNavigation } from '@react-navigation/native';
import config from '../../config';
import eventStyles from '../../styles/eventStyles';

export default function EventCard({ event }: { event: Event }) {
  const navigation = useNavigation<any>();

  const imgSrc = event.logoPath ? `${config.apiBaseUrl}/event/images/${event.logoPath.split('/').pop()}` : null;

  return (
    <Pressable onPress={() => navigation.navigate('Event', { event })} style={[eventStyles.eventCard, { backgroundColor: event.mainColorHex }]}>
      <Text numberOfLines={2} style={eventStyles.eventCardTitle}>{event.title}</Text>
            
      <View style={eventStyles.eventCardContentRow}>
        {imgSrc ? 
          <Image 
            source={{ uri: imgSrc }} 
            style={eventStyles.eventCardImage}
          /> 
        : 
          <Text style={eventStyles.eventCardNoImage}>No Logo</Text>
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