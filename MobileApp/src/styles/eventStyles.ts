import { StyleSheet } from 'react-native';
import Colors from '../enums/colors';

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
  eventsLoadPastEvents: {
    padding: 14,
    borderRadius: 8,
    marginBottom: 20,
    width: '90%',
  },
  eventsLoadPastEventsText: {
    color: 'white',
    fontWeight: 500,
    fontSize: 16,
    textAlign: 'center',
  },
  loadingContainer: {
    display: 'flex',
    flexDirection: 'column',
    justifyContent: 'center',
    alignItems: 'center',
    gap: 12,
    height: '100%',
  },
  loadingText: {
    fontSize: 16,
    color: '#667',
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
    width: '60%',
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
    width: 44,
    fontSize: 48, 
    fontWeight: 'bold'
  },
  eventContentContainer: {
    display: 'flex',
    gap: 8,
  },
  eventPageHeader: {
    display: 'flex',
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    gap: 12,
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
  eventPageScheduleButton: {
    padding: 12,
    borderRadius: 8,
    backgroundColor: Colors.DEFAULT_BUTTON_COLOR,
    alignItems: 'center',
    marginVertical: 10,
  },
  eventPageScheduleButtonText: {
    color: 'white',
    fontWeight: 'bold',
  },
});

export default eventStyles;