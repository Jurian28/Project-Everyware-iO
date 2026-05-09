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


type Tag = {
	idTag: string;
	title: string;
	colorHex: string;
};

type Room = {
	roomLabel: string;
};

type Session = {
	title: string;
	startTime: string;
	endTime: string;
	room?: Room;
	tags?: Tag[];
	speakerName?: string;
	description?: string;
};


const formatTimeRange = (start: string, end: string) => {
	const fmt = (d: Date) =>
		d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });

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

function Section({ title, children }: { title: string; children: React.ReactNode }) {
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
			<Text style={{ marginTop: 10 }}>Loading session...</Text>
		</View>
	);
}

function ErrorState({ message }: { message: string }) {
	return (
		<View style={styles.center}>
			<Text style={{ color: 'red' }}>{message}</Text>
		</View>
	);
}


export default function SessionViewPage() {
	const route = useRoute<any>();
	const navigation = useNavigation<any>();
	const { eventId, sessionId } = route.params;

	const [session, setSession] = useState<Session | null>(null);
	const [loading, setLoading] = useState(true);
	const [error, setError] = useState<string | null>(null);

	useEffect(() => {
		let mounted = true;

		const load = async () => {
			try {
				setLoading(true);
				setError(null);

				const data = await SessionService.getSession(eventId, sessionId);
				if (mounted) setSession(data);
			} catch (e: any) {
				if (mounted) setError(e?.message ?? 'Something went wrong');
			} finally {
				if (mounted) setLoading(false);
			}
		};

		load();
		return () => {
			mounted = false;
		};
	}, [eventId, sessionId]);

	if (loading) return <Loading />;
	if (error) return <ErrorState message={error} />;
	if (!session) return <ErrorState message="Session not found" />;

	return (
		<ScrollView contentContainerStyle={styles.container}>

			<TouchableOpacity
				onPress={() => navigation.goBack()}
				style={styles.backButton}
			>
				<Text style={styles.backText}>← Back</Text>
			</TouchableOpacity>

			<Text style={styles.title}>{session.title}</Text>

			<Text style={styles.time}>
				{formatTimeRange(session.startTime, session.endTime)}
			</Text>

			{session.room?.roomLabel && (
				<Text style={styles.location}>{session.room.roomLabel}</Text>
			)}

			{!!session.tags?.length && (
				<View style={styles.tagRow}>
					{session.tags.map((tag) => (
						<TagItem key={tag.idTag} tag={tag} />
					))}
				</View>
			)}

			<Section title="Sprekers">
				<Text style={styles.speakerName}>
					{session.speakerName ?? 'TBA'}
				</Text>
			</Section>

			<Section title="About">
				<Text style={styles.aboutText}>
					{session.description ?? 'No description available yet.'}
				</Text>
			</Section>
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
		gap: 8,
		marginBottom: 10,
	},

	tag: {
		paddingHorizontal: 10,
		paddingVertical: 4,
		borderRadius: 20,
		borderWidth: 1,
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
});
