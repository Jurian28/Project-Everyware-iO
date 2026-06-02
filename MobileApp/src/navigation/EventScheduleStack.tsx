import React, { useEffect } from 'react';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { useRoute, useNavigation } from '@react-navigation/native';

import EventSchedule from '../pages/events/EventSchedule';
import SessionViewPage from '../pages/sessions/session';
import QRCodePage from '../pages/sessions/QRCodePage';

const Stack = createNativeStackNavigator();

export default function EventScheduleStack() {
	const route = useRoute<any>();
	const params = route.params || {};

	return (
		<Stack.Navigator
			screenOptions={{
				headerShown: false,
			}}
			initialRouteName="EventSchedule"
			key={params.eventId}
		>
			<Stack.Screen
				name="EventSchedule"
				component={EventSchedule}
				initialParams={params}
			/>
			<Stack.Screen
				name="SessionView"
				component={SessionViewPage}
				initialParams={params}
			/>
			<Stack.Screen
				name="SessionQRCode"
				component={QRCodePage}
				initialParams={params}
			/>
		</Stack.Navigator>
	);
}
