import { StyleSheet } from 'react-native';

const eventStyles = StyleSheet.create({
  // Overview Page Styles
  scrollViewContainer: {
    flex: 1,
  },
  eventsContainer: {
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    gap: 12,
  },
  eventsNoEvents: {
    fontSize: 16,
    fontWeight: 'bold',
  },
  eventsNoEventsContainer: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    height: 100,
  },
  eventsListContainer: {
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    gap: 12,
    width: '100%',
  },
  eventsTitle: {
    fontSize: 32,
    fontWeight: 'bold',
    marginBottom: 24,
    marginTop: 24,
  },
  
  // Event Card Styles
  eventCard: {
    padding: 12,
    marginBottom: 12,
    borderRadius: 8,
    width: '90%',
    maxHeight: 200,
  },
  eventCardTitle: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 8,
  },
  eventCardContentRow: {
    display: 'flex',
    flexDirection: 'row',
    alignItems: 'center',
    gap: 24,
  },
  eventCardImage: {
    width: 100,
    height: 100,
    marginBottom: 8,
  },
  eventCardNoImage: {
    minWidth: 100,
    minHeight: 100,
    marginBottom: 8,
    backgroundColor: '#ccc',
    textAlign: 'center',
    lineHeight: 100,
  },
  eventCardTextSection: {
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'space-between',
    gap: 16,
    width: '65%',
  },
  eventCardDescription: {
    marginBottom: 8,
  },
  eventCardDate: {
    fontSize: 12,
    marginBottom: 12,
    fontWeight: 'bold',
  },

  // Event Page Styles
  eventPageContainer: {
    padding: 12,
  },
  backButton: {
    // padding: 4,
    // marginBottom: 4,
    width: 44,
    fontSize: 48, 
    fontWeight: 'bold'
  },
  eventContentContainer: {
    display: 'flex',
    gap: 8,
  },
  eventPageTitle: {
    fontSize: 24,
    fontWeight: 'bold',
    marginBottom: 12,
  },
  eventImageContainer: {
    display: 'flex',
    justifyContent: 'center',
    alignItems: 'center',
    marginBottom: 4,
  },
  eventPageImage: {
    width: 150,
    height: 150,
    marginBottom: 8,
  },
  eventPageNoImage: {
    width: 150,
    height: 150,
    marginBottom: 8,
    backgroundColor: '#ccc',
    textAlign: 'center',
    lineHeight: 150,
  },
  eventPageMeta: {
    fontSize: 14,
    marginBottom: 8,
    fontWeight: 'bold',
    color: '#667',
  },
  eventPageDivider: {
    height: 1,
    marginVertical: 10,
  },
  eventPageDescriptionSection: {
    display: 'flex',
    gap: 4,
  },
  eventPageDescriptionTitle: {
    fontSize: 16,
    fontWeight: 'bold',
  },
  eventPageDescriptionText: {
    fontSize: 14,
  },
});

export default eventStyles;