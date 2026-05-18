import React from 'react';
import { createNativeStackNavigator } from '@react-navigation/native-stack';

import SessionOverview from '../pages/sessions/overview';
import SessionViewPage from '../pages/sessions/session';

const Stack = createNativeStackNavigator();

export default function SessionStack() {
	return (
		<Stack.Navigator
		screenOptions={{
			headerShown: false,
		}}
		>
		<Stack.Screen
		name="SessionOverview"
		component={SessionOverview}
		/>
		<Stack.Screen
		name="SessionView"
		component={SessionViewPage}
		/>
		</Stack.Navigator>
	);
}
