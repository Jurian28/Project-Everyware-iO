import { StyleSheet } from 'react-native';

const authStyles = StyleSheet.create({
  root: {
    flex: 1,
    backgroundColor: '#f2f5f9',
    justifyContent: 'center',
    padding: 20,
  },
  card: {
    backgroundColor: '#ffffff',
    borderRadius: 12,
    padding: 20,
    borderWidth: 1,
    borderColor: '#d5dbe5',
    gap: 10,
  },
  title: {
    color: '#1d2533',
    fontSize: 24,
    fontWeight: '700',
  },
  subtitle: {
    color: '#5a6475',
    marginBottom: 8,
  },
  label: {
    color: '#2a3344',
    fontWeight: '600',
    marginTop: 6,
  },
  input: {
    borderWidth: 1,
    borderColor: '#bec7d4',
    backgroundColor: '#f9fbff',
    borderRadius: 8,
    paddingHorizontal: 12,
    paddingVertical: 10,
    color: '#172033',
  },
  button: {
    backgroundColor: '#1e4ea3',
    borderRadius: 8,
    alignItems: 'center',
    paddingVertical: 12,
    marginTop: 14,
  },
  buttonPressed: {
    opacity: 0.9,
  },
  buttonDisabled: {
    opacity: 0.65,
  },
  buttonText: {
    color: '#ffffff',
    fontWeight: '700',
    fontSize: 15,
  },
  linkRow: {
    flexDirection: 'row',
    justifyContent: 'center',
    alignItems: 'center',
    gap: 6,
    marginTop: 8,
  },
  linkLabel: {
    color: '#5a6475',
  },
  linkText: {
    color: '#1e4ea3',
    fontWeight: '700',
  },
});

export default authStyles;
