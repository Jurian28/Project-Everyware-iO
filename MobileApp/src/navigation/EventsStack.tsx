import { createNativeStackNavigator } from '@react-navigation/native-stack';
import EventsOverview from '../pages/events/Overview';
import EventPage from '../pages/events/Event';
import EventSchedule from '../pages/events/EventSchedule';
import SessionViewPage from '../pages/sessions/session';
import QRCodePage from '../pages/sessions/QRCodePage';

const Stack = createNativeStackNavigator();

export default function EventsStack() {
  return (
    <Stack.Navigator screenOptions={{ headerShown: false }}>
      <Stack.Screen name="EventsOverview" component={EventsOverview} />
      <Stack.Screen name="Event" component={EventPage} />
      <Stack.Screen name="EventSchedule" component={EventSchedule} />
      <Stack.Screen name="SessionView" component={SessionViewPage} />
      <Stack.Screen name="SessionQRCode" component={QRCodePage} />
    </Stack.Navigator>
  );
}