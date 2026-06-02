import { createNativeStackNavigator } from '@react-navigation/native-stack';
import EventsOverview from '../pages/events/Overview';
import EventPage from '../pages/events/Event';
import EventScheduleStack from './EventScheduleStack';

const Stack = createNativeStackNavigator();

export default function EventsStack() {
  return (
    <Stack.Navigator screenOptions={{ headerShown: false }}>
      <Stack.Screen name="EventsOverview" component={EventsOverview} />
      <Stack.Screen name="Event" component={EventPage} />
      <Stack.Screen name="EventSchedule" component={EventScheduleStack} />
    </Stack.Navigator>
  );
}