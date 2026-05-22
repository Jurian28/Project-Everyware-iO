import { StyleSheet } from 'react-native';


const sessionConflictModalStyles = StyleSheet.create({
})

const sessionStyles = StyleSheet.create({
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


	// conflictModal styles:
	sessionCard: {
		backgroundColor: '#f3f4f6',
		padding: 12,
		borderRadius: 10,
		marginTop: 10,
		marginBottom: 16,
	},

	sessionTitle: {
		fontSize: 15,
		fontWeight: '700',
		marginBottom: 4,
		color: '#111827',
	},

	sessionMeta: {
		fontSize: 13,
		color: '#4b5563',
	},

});

export default sessionStyles;
