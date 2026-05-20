import React from 'react';
import {
	Modal,
	View,
	Text,
	Pressable,
} from 'react-native';

import sessionStyles from '../../styles/sessionStyles';


type ConflictSession = {
	idSession: number;
	title: string;
	startTime: string;
	endTime: string;
};

type EnrollConflictModalProps = {
	visible: boolean;
	conflictSession: ConflictSession | null;
	onCancel: () => void;
	onConfirm: () => void;
};

export default function EnrollConflictModal({
	visible,
	conflictSession,
	onCancel,
	onConfirm,
}: EnrollConflictModalProps) {

	const formatTimeRange = (
		start: string,
		end: string
	) => {
		const fmt = (d: Date) =>
		d.toLocaleTimeString([], {
			hour: '2-digit',
			minute: '2-digit',
		});

		return `${fmt(new Date(start))} - ${fmt(
			new Date(end)
		)}`;
	};


	return (
		<Modal
		visible={visible}
		transparent
		animationType="fade"
		onRequestClose={onCancel}
		>
		<View
		style={{
			flex: 1,
			backgroundColor: 'rgba(0,0,0,0.5)',
			justifyContent: 'center',
			padding: 20,
		}}
		>
		<View
		style={{
			backgroundColor: 'white',
			padding: 20,
			borderRadius: 12,
		}}
		>
		<Text
		style={{
			fontSize: 18,
			fontWeight: 'bold',
			marginBottom: 10,
		}}
		>
		Session Conflict
		</Text>

		<Text>
		You are already enrolled in:
			</Text>

		{conflictSession && (
			<View style={sessionStyles.sessionCard}>
			<Text style={sessionStyles.sessionTitle}>
			{conflictSession.title}
			</Text>

			<Text style={sessionStyles.sessionMeta}>
			{formatTimeRange(
				conflictSession.startTime,
				conflictSession.endTime
			)}
			</Text>
			</View>
		)}

		<View
		style={{
			flexDirection: 'row',
			justifyContent: 'space-between',
		}}
		>
		<Pressable onPress={onCancel}>
		<Text style={{ color: 'red' }}>
		Cancel
		</Text>
		</Pressable>

		<Pressable onPress={onConfirm}>
		<Text
		style={{
			color: 'blue',
			fontWeight: 'bold',
		}}
		>
		Unenroll & Join
		</Text>
		</Pressable>
		</View>
		</View>
		</View>
		</Modal>
	);
}
