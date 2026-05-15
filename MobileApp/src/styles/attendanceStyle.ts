import { StyleSheet } from 'react-native';
import colors from '../enums/colors';

export default StyleSheet.create({
  attendanceContainer: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    padding: 16,
  }
});

export const qrCodeStyles = StyleSheet.create({
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
  errorIcon: {
    fontSize: 40,
    marginBottom: 8,
  },
  errorText: {
    fontSize: 15,
    textAlign: 'center',
    opacity: 0.8,
    marginBottom: 8,
  },
  bottomOverlay: {
    flex: 1,
    alignItems: 'center',
    paddingTop: 28,
    gap: 12,
  },
  hint: {
    fontSize: 15,
    opacity: 0.85,
    letterSpacing: 0.3,
  },
  button: {
    backgroundColor: colors.DEFAULT_BUTTON_COLOR,
    paddingHorizontal: 28,
    paddingVertical: 12,
    borderRadius: 8,
    marginTop: 4,
  },
  buttonText: {
    color: '#FFF',
    fontWeight: '700',
    fontSize: 15,
  },
  closeButton: {
    paddingHorizontal: 20,
    paddingVertical: 10,
  },
  closeText: {
    color: 'rgba(255,255,255,0.5)',
    fontSize: 14,
  },
});