import { StyleSheet } from 'react-native';

export const toastStyles = StyleSheet.create({
  toastStyle: {
    backgroundColor: '#fff',
    borderRadius: 8,
    padding: 16,
    width: 320,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 5,
  },
  headerStyle: {
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  titleStyle: {
    fontSize: 18,
    fontWeight: '600',
    marginBottom: 8,
  },
  contentStyle: {
    fontSize: 14,
    color: '#4b5563',
  },
  closeButtonStyle: {
    padding: 4,
  },
});
