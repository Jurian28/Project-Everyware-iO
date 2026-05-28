import React, { useEffect } from 'react';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { useRoute, useNavigation } from '@react-navigation/native';

import EventSchedule from '../pages/events/EventSchedule';
import SessionViewPage from '../pages/sessions/session';

const Stack = createNativeStackNavigator();

export default function EventScheduleStack() {
	const route = useRoute<any>();
	const navigation = useNavigation<any>();
	const params = route.params || {};

	useEffect(() => {
		if (Object.keys(params).length > 0) {
			navigation.setParams(params);
		}
	}, [JSON.stringify(params)]);

	return (
		<Stack.Navigator
			screenOptions={{
				headerShown: false,
			}}
			initialRouteName="EventSchedule"
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
		</Stack.Navigator>
	);
}
