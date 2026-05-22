import { StyleSheet } from 'react-native';
import colors from '../enums/colors';

export default StyleSheet.create({
  attendanceContainer: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    padding: 16,
  },
  container: {
    flex: 1,
    position: 'relative' as any,
    overflow: 'hidden' as any,
  },
  centered: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#0A0A0A',
    padding: 24,
    gap: 12,
  },
   hint: {
    fontSize: 15,
    opacity: 0.85,
    letterSpacing: 0.3,
    color: '#FFF',
  },
});