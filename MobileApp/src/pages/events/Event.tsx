import { useNavigation, useRoute, type NavigationProp, type ParamListBase, type RouteProp } from '@react-navigation/native';
import { useEffect } from 'react';
import { Image, Pressable, Text, View } from 'react-native';
import config from '../../config';
import { useEventContext } from '../../context/EventContext';
import { Event } from '../../services/EventService';
import eventStyles from '../../styles/eventStyles';

type EventPageParams = { event: Event };

function Header({ navigation }: { navigation: NavigationProp<ParamListBase> }) {
  return (
    <View style={eventStyles.eventPageHeader}>
      <Pressable
        onPress={() => navigation.goBack()}
        style={eventStyles.backButton}
      >
        <Text style={eventStyles.backButton}>&larr;</Text>
      </Pressable>
    </View>
  );
}

export default function EventPage() {
  const route = useRoute<RouteProp<{ EventPage: EventPageParams }, 'EventPage'>>();
  const { event } = route.params;
  const navigation = useNavigation<NavigationProp<ParamListBase>>();
  const { setCurrentEventName } = useEventContext();

  useEffect(() => {
    setCurrentEventName(event.title);
    return () => setCurrentEventName(null);
  }, [event.title]);

  const imgSrc = event.logoPath
    ? `${config.apiBaseUrl}/event${event.logoPath}`
    : null;

  return (
    <View style={eventStyles.eventPageContainer}>
      <Header navigation={navigation} />

      <View style={eventStyles.eventContentContainer}>
        <Text numberOfLines={2} style={eventStyles.eventPageTitle}>
          {event.title}
        </Text>

        <View style={eventStyles.eventImageContainer}>
          {imgSrc ? (
            <Image
              source={{ uri: imgSrc }}
              style={eventStyles.eventPageImage}
            />
          ) : (
            <Text style={eventStyles.eventPageNoImage}>No Logo</Text>
          )}
        </View>

        <View>
          <Text style={eventStyles.eventPageMeta}>
            {event.startDate.toLocaleDateString()} -{' '}
            {event.endDate.toLocaleDateString()}
          </Text>

          {event.location && (
            <Text style={eventStyles.eventPageMeta}>{event.location}</Text>
          )}
        </View>

        <View
          style={[
            eventStyles.eventPageDivider,
            { backgroundColor: event.mainColorHex },
          ]}
        />

        <Pressable
          onPress={() =>
            navigation.navigate('EventSchedule', {
              eventId: event.idEvent.toString(),
              eventTitle: event.title,
              eventColor: event.mainColorHex,
              eventAccentColor: event.accentColorHex,
            })
          }
          style={eventStyles.eventPageScheduleButton}
        >
          <Text style={eventStyles.eventPageScheduleButtonText}>
            View Schedule
          </Text>
        </Pressable>

        {event.description && (
          <View style={eventStyles.eventPageDescriptionSection}>
            <Text style={eventStyles.eventPageDescriptionTitle}>
              Description
            </Text>
            <Text style={eventStyles.eventPageDescriptionText}>
              {event.description}
            </Text>
          </View>
        )}
      </View>
    </View>
  );
}
