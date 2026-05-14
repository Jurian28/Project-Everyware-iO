import React from 'react';
import { View, Text, Button } from 'react-native';
import { useNavigation } from '@react-navigation/native';

export default function SessionOverview() {
	const navigation = useNavigation<any>();

	return (
		<View style={{ padding: 20 }}>
		<Text>Session Overview</Text>

		<Button
		title="Go to Session View"
		onPress={() =>
			navigation.navigate('SessionView', {
				sessionId: 1,
			})
		}
		/>



		<Button
		title="Go to Session View 2"
		onPress={() =>
			navigation.navigate('SessionView', {
				sessionId: 2,
			})
		}
		/>
		</View>
	);
}
