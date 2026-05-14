import React, { useEffect, useState } from 'react';
import {
	View,
	Text,
	ActivityIndicator,
	StyleSheet,
	ScrollView,
	TouchableOpacity,
} from 'react-native';
import { useRoute, useNavigation } from '@react-navigation/native';

import SessionService from '../../services/SessionService';
import EnrollConflictModal from '../../components/sessions/enrollConflictModal';

type Tag = {
	idTag: number;
	title: string;
	colorHex: string;
};

type Room = {
	roomLabel: string;
	capacity: number;
};

type ConflictSession = {
	idSession: number;
	title: string;
	startTime: string;
	endTime: string;
};

type Session = {
	sessionId: number;
	title: string;
	startTime: string;
	endTime: string;
	room?: Room;
	tags?: Tag[];
	speakerName?: string;
	description?: string;
	placesLeft: number;
	isEnrolled?: boolean;
};

const formatTimeRange = (start: string, end: string) => {
	const fmt = (d: Date) =>
	d.toLocaleTimeString([], {
		hour: '2-digit',
		minute: '2-digit',
	});

	return `${fmt(new Date(start))} - ${fmt(new Date(end))}`;
};

function TagItem({ tag }: { tag: Tag }) {
	return (
		<View style={[styles.tag, { borderColor: tag.colorHex }]}>
		<Text style={[styles.tagText, { color: tag.colorHex }]}>
		{tag.title}
		</Text>
		</View>
	);
}

function Section({
	title,
	children,
}: {
	title: string;
	children: React.ReactNode;
}) {
	return (
		<>
		<Separator />
		<Text style={styles.sectionTitle}>{title}</Text>
		{children}
		</>
	);
}

function Separator() {
	return <View style={styles.separator} />;
}

function Loading() {
	return (
		<View style={styles.center}>
		<ActivityIndicator size="large" />
		<Text style={{ marginTop: 10 }}>
		Loading session...
			</Text>
		</View>
	);
}

function ErrorState({ message }: { message: string }) {
	return (
		<View style={styles.center}>
		<Text style={{ color: 'red' }}>
		{message}
		</Text>
		</View>
	);
}

function PlacesLeft({
	placesLeft,
	isEnrolled,
	onEnroll,
	onWithdraw,
	loading,
}: {
	placesLeft: number;
	isEnrolled: boolean;
	onEnroll?: () => void;
	onWithdraw?: () => void;
	loading?: boolean;
}) {
	return (
		<View style={styles.enrollRow}>
		<Text style={styles.spotsInlineText}>
		{placesLeft} spaces left
		</Text>

		<TouchableOpacity
		disabled={loading}
		onPress={isEnrolled ? onWithdraw : onEnroll}
		style={[
			styles.actionButton,
			isEnrolled
				? styles.withdrawButton
				: styles.enrollButton,
				loading && styles.disabledButton,
		]}
		>
		<Text style={styles.actionText}>
		{loading
			? 'Loading...'
			: isEnrolled
				? 'Withdraw from session'
				: 'Enroll for session'}
				</Text>
				</TouchableOpacity>
				</View>
	);
}

export default function SessionViewPage() {
	const route = useRoute<any>();
	const navigation = useNavigation<any>();

	const { sessionId } = route.params;

	const [session, setSession] =
		useState<Session | null>(null);

	const [loading, setLoading] = useState(true);

	const [error, setError] =
		useState<string | null>(null);

	const [actionLoading, setActionLoading] =
		useState(false);

	const [conflictVisible, setConflictVisible] =
		useState(false);

	const [conflictSession, setConflictSession] =
		useState<ConflictSession | null>(null);

	const loadSession = async () => {
		try {
			setLoading(true);
			setError(null);

			const data =
				await SessionService.getSession(
					sessionId
			);

			setSession(data);
		} catch (e: any) {
			setError(
				e?.message ?? 'Something went wrong'
			);
		} finally {
			setLoading(false);
		}
	};

	useEffect(() => {
		loadSession();
	}, [sessionId]);

	const handleEnroll = async () => {
		if (!session) return;

		try {
			setActionLoading(true);

			const res =
				await SessionService.enroll(
					session.sessionId
			);

			if (res?.type === 'conflict') {
				setConflictSession(res.conflictSession);
				setConflictVisible(true);
				return;
			}

			await loadSession();
		} catch (e: any) {
			alert(e.message ?? 'Enroll failed');
		} finally {
			setActionLoading(false);
		}
	};

	const handleWithdraw = async () => {
		if (!session) return;

		try {
			setActionLoading(true);

			await SessionService.withdraw(
				session.sessionId
			);

			await loadSession();
		} catch (e: any) {
			alert(e.message ?? 'Withdraw failed');
		} finally {
			setActionLoading(false);
		}
	};

	const confirmOverride = async () => {
		if (!session || !conflictSession)
			return;

		try {
			setActionLoading(true);

			// withdraw old conflicting session
			await SessionService.withdraw(
				conflictSession.idSession
			);

			// enroll current session
			await SessionService.enroll(
				session.sessionId
			);

			await loadSession();
		} catch (e: any) {
			alert(
				e.message ??
					'Failed to switch sessions'
			);
		} finally {
			setActionLoading(false);
			setConflictVisible(false);
		}
	};

	if (loading) return <Loading />;

	if (error)
		return <ErrorState message={error} />;

	if (!session)
		return (
			<ErrorState message="Session not found" />
		);

		return (
			<ScrollView
			contentContainerStyle={styles.container}
			>
			<TouchableOpacity
			onPress={() => navigation.goBack()}
			style={styles.backButton}
			>
			<Text style={styles.backText}>
			← Back
			</Text>
			</TouchableOpacity>

			<Text style={styles.title}>
			{session.title}
			</Text>

			<Text style={styles.time}>
			{formatTimeRange(
				session.startTime,
				session.endTime
			)}
			</Text>

			{session.room?.roomLabel && (
				<Text style={styles.location}>
				{session.room.roomLabel}
				</Text>
			)}

			{!!session.tags?.length && (
				<View style={styles.tagRow}>
				{session.tags.map((tag) => (
					<TagItem
					key={tag.idTag}
					tag={tag}
					/>
				))}
				</View>
			)}

			<PlacesLeft
			placesLeft={session.placesLeft}
			isEnrolled={
				session.isEnrolled ?? false
			}
			onEnroll={handleEnroll}
			onWithdraw={handleWithdraw}
			loading={actionLoading}
			/>

			<Section title="Speakers">
			<Text style={styles.speakerName}>
			{session.speakerName ??
				'TBA'}
			</Text>
			</Section>

			<Section title="About">
			<Text style={styles.aboutText}>
			{session.description ??
				'No description available yet.'}
			</Text>
			</Section>

			<EnrollConflictModal
			visible={conflictVisible}
			conflictSession={conflictSession}
			onCancel={() =>
				setConflictVisible(false)
			}
			onConfirm={confirmOverride}
			/>
			</ScrollView>
		);
}

const styles = StyleSheet.create({
	container: {
		padding: 20,
		paddingBottom: 40,
	},

	center: {
		flex: 1,
		alignItems: 'center',
		justifyContent: 'center',
	},

	backButton: {
		marginBottom: 10,
	},

	backText: {
		fontSize: 16,
		color: '#007AFF',
	},

	title: {
		fontSize: 26,
		fontWeight: '700',
		marginBottom: 6,
	},

	time: {
		fontSize: 15,
		color: '#444',
		marginBottom: 4,
	},

	location: {
		fontSize: 14,
		color: '#666',
		marginBottom: 10,
	},

	tagRow: {
		flexDirection: 'row',
		flexWrap: 'wrap',
	},

	tag: {
		paddingHorizontal: 10,
		paddingVertical: 4,
		borderRadius: 20,
		borderWidth: 1,
		marginRight: 8,
	},

	tagText: {
		fontSize: 12,
		fontWeight: '600',
	},

	separator: {
		height: 1,
		backgroundColor: '#e6e6e6',
		marginVertical: 16,
	},

	sectionTitle: {
		fontSize: 16,
		fontWeight: '700',
		marginBottom: 10,
	},

	speakerName: {
		fontSize: 16,
		fontWeight: '600',
		color: '#222',
	},

	aboutText: {
		fontSize: 14,
		color: '#333',
		lineHeight: 20,
	},

	enrollRow: {
		flexDirection: 'row',
		justifyContent: 'space-between',
		alignItems: 'center',
		paddingVertical: 8,
	},

	actionButton: {
		paddingVertical: 8,
		paddingHorizontal: 16,
		borderRadius: 10,
	},

	enrollButton: {
		backgroundColor: '#3b82f6',
	},

	withdrawButton: {
		backgroundColor: '#ef4444',
	},

	disabledButton: {
		backgroundColor: '#374151',
	},

	actionText: {
		color: 'white',
		fontWeight: '700',
		fontSize: 13,
	},

	spotsInlineText: {
		fontSize: 12,
		color: '#666',
		textTransform: 'uppercase',
		letterSpacing: 0.5,
		fontWeight: 'bold',
	},
});
