import { StyleSheet, Dimensions } from 'react-native';

const { width } = Dimensions.get('window');
export const TIMELINE_OFFSET_LEFT = 55; // Space for the hour labels
export const HOUR_HEIGHT = 120; // Height in pixels for 1 hour
export const MINUTE_HEIGHT = HOUR_HEIGHT / 60; // Height in pixels for 1 minute

const scheduleStyles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#FAFAFA',
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    paddingHorizontal: 16,
    paddingVertical: 12,
    backgroundColor: '#FFFFFF',
    borderBottomWidth: 1,
    borderBottomColor: '#E2E8F0',
  },
  headerTitle: {
    fontSize: 18,
    fontWeight: '600',
    color: '#1E293B',
  },
  monthTitle: {
    fontSize: 14,
    color: '#64748B',
    marginTop: 2,
  },
  navButton: {
    padding: 8,
  },
  scrollView: {
    flex: 1,
  },
  timelineContainer: {
    flexDirection: 'row',
    paddingBottom: 40,
    marginTop: 10,
    minHeight: 24 * HOUR_HEIGHT,
  },
  timeLabelContainer: {
    width: TIMELINE_OFFSET_LEFT,
    alignItems: 'flex-end',
    paddingRight: 8,
  },
  timeLabel: {
    fontSize: 12,
    color: '#94A3B8',
    fontWeight: '500',
    marginTop: -8, // Center text physically on the top line of the hour block
  },
  timeRowLine: {
    flex: 1,
    borderTopWidth: 1,
    borderTopColor: '#E2E8F0',
  },
  hourRow: {
    height: HOUR_HEIGHT,
    flexDirection: 'row',
    width: '100%',
  },
  cardsContainer: {
    flex: 1,
    position: 'relative',
    marginRight: 10, // Margin on right
  },
  sessionCard: {
    position: 'absolute',
    backgroundColor: '#FFFFFF',
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#E2E8F0',
    borderLeftWidth: 4,
    padding: 8,
    overflow: 'hidden',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 2,
    elevation: 2,
    flexDirection: 'column',
    justifyContent: 'space-between',
  },
  tagContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    marginTop: 'auto',
    gap: 4,
  },
  tagBadge: {
    paddingHorizontal: 4,
    paddingVertical: 2,
    borderRadius: 4,
    borderWidth: 1,
  },
  tagText: {
    fontSize: 10,
    fontWeight: '600',
  },
  sessionTitle: {
    fontSize: 14,
    fontWeight: '700',
    color: '#0F172A',
    marginBottom: 4,
  },
  sessionSubtitle: {
    fontSize: 12,
    color: '#475569',
    marginBottom: 2,
  },
  currentTimeLineContainer: {
    position: 'absolute',
    left: TIMELINE_OFFSET_LEFT,
    right: 0,
    flexDirection: 'row',
    alignItems: 'center',
    zIndex: 10,
  },
  currentTimeCircle: {
    width: 8,
    height: 8,
    borderRadius: 4,
    backgroundColor: '#EF4444',
    marginLeft: -4,
  },
  currentTimeLine: {
    flex: 1,
    height: 2,
    backgroundColor: '#EF4444',
  },
});

export default scheduleStyles;
