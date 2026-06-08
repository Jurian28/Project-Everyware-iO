import { Pressable, Text, View, Image } from 'react-native';
import { Event } from '../../services/EventService';
import { useNavigation } from '@react-navigation/native';
import config from '../../config';
import eventStyles from '../../styles/eventStyles';
import { getTextColorForBackground } from '../../utils/colors';

export default function EventCard({ event }: { event: Event }) {
  const navigation = useNavigation<any>();
  
  const imgSrc = event.logoPath ? `${config.apiBaseUrl}/event${event.logoPath}` : null;

  const textColor = getTextColorForBackground(event.mainColorHex || '#FFFFFF');

  return (
    <Pressable onPress={() => navigation.navigate('Event', { event })} style={[eventStyles.eventCard, { backgroundColor: event.mainColorHex }]}>
      <Text numberOfLines={2} style={[eventStyles.eventCardTitle, { color: textColor }]}>{event.title}</Text>
            
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
            <Text numberOfLines={3} style={[eventStyles.eventCardDescription, { color: textColor }]}>{event.description}</Text>
          )}
          
          <Text style={[eventStyles.eventCardDate, { color: textColor }]}>
            {event.startDate.toLocaleDateString()} - {event.endDate.toLocaleDateString()}
          </Text>
        </View>
      </View>
    </Pressable>
  )
}