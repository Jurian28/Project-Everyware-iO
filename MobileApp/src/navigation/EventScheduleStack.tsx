import React from 'react';
import { createNativeStackNavigator } from '@react-navigation/native-stack';

import EventSchedule from '../pages/events/EventSchedule';
import SessionViewPage from '../pages/sessions/session';

const Stack = createNativeStackNavigator();

export default function EventScheduleStack() {
	return (
		<Stack.Navigator
		screenOptions={{
			headerShown: false,
		}}
		>
		<Stack.Screen
		name="EventSchedule"
		component={EventSchedule}
		/>
		<Stack.Screen
		name="SessionView"
		component={SessionViewPage}
		/>
		</Stack.Navigator>
	);
}
