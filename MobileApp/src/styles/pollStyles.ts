import { StyleSheet } from 'react-native';
import colors from '../enums/colors';

const pollStyles = StyleSheet.create({
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

  header: {
    display: 'flex',
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 16,
  },

  headerTitle: {
    fontSize: 24,
    fontWeight: '700',
  },

  pollCard: {
    backgroundColor: '#f8f9fa',
    borderRadius: 12,
    padding: 16,
    marginBottom: 12,
    borderWidth: 1,
    borderColor: '#e6e6e6',
  },

  pollTitle: {
    fontSize: 18,
    fontWeight: '700',
    marginBottom: 4,
    color: '#111827',
  },

  pollDescription: {
    fontSize: 14,
    color: '#6b7280',
    marginBottom: 8,
  },

  pollMeta: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 8,
  },

  statusBadge: {
    paddingHorizontal: 10,
    paddingVertical: 4,
    borderRadius: 12,
    fontSize: 12,
    fontWeight: '600',
    overflow: 'hidden',
  },

  statusOpen: {
    backgroundColor: '#d1fae5',
    color: '#065f46',
  },

  statusClosed: {
    backgroundColor: '#e5e7eb',
    color: '#374151',
  },

  answerItem: {
    paddingVertical: 6,
    paddingHorizontal: 12,
    backgroundColor: '#ffffff',
    borderRadius: 8,
    marginBottom: 4,
    borderWidth: 1,
    borderColor: '#e6e6e6',
  },

  answerText: {
    fontSize: 14,
    color: '#374151',
  },

  answersList: {
    marginTop: 4,
  },

  actionRow: {
    flexDirection: 'row',
    justifyContent: 'flex-end',
    marginTop: 8,
    gap: 8,
  },

  button: {
    paddingVertical: 8,
    paddingHorizontal: 16,
    borderRadius: 8,
  },

  closeButton: {
    backgroundColor: colors.DEFAULT_BUTTON_COLOR,
  },

  deleteButton: {
    backgroundColor: '#ef4444',
  },

  buttonText: {
    color: 'white',
    fontWeight: '700',
    fontSize: 13,
  },

  disabledButton: {
    backgroundColor: '#374151',
  },

  createButton: {
    backgroundColor: colors.DEFAULT_BUTTON_COLOR,
    paddingVertical: 12,
    paddingHorizontal: 24,
    borderRadius: 10,
    alignItems: 'center',
    marginTop: 8,
  },

  createButtonText: {
    color: 'white',
    fontWeight: '700',
    fontSize: 16,
  },

  emptyText: {
    color: '#64748B',
    fontSize: 16,
    textAlign: 'center',
    marginTop: 40,
  },

  formGroup: {
    marginBottom: 16,
  },

  label: {
    fontSize: 14,
    fontWeight: '600',
    color: '#374151',
    marginBottom: 6,
  },

  input: {
    borderWidth: 1,
    borderColor: '#d1d5db',
    borderRadius: 8,
    padding: 12,
    fontSize: 16,
    color: '#111827',
    backgroundColor: '#ffffff',
  },

  textArea: {
    minHeight: 80,
    textAlignVertical: 'top',
  },

  answerInputRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 6,
    gap: 8,
  },

  answerInput: {
    flex: 1,
    borderWidth: 1,
    borderColor: '#d1d5db',
    borderRadius: 8,
    padding: 10,
    fontSize: 14,
    color: '#111827',
    backgroundColor: '#ffffff',
  },

  removeButton: {
    padding: 8,
    borderRadius: 8,
    backgroundColor: '#fee2e2',
  },

  removeButtonText: {
    color: '#ef4444',
    fontWeight: '600',
    fontSize: 12,
  },

  addButton: {
    paddingVertical: 8,
    paddingHorizontal: 16,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: colors.DEFAULT_BUTTON_COLOR,
    alignItems: 'center',
    marginTop: 4,
  },

  addButtonText: {
    color: colors.DEFAULT_BUTTON_COLOR,
    fontWeight: '600',
    fontSize: 14,
  },

  submitButton: {
    backgroundColor: colors.DEFAULT_BUTTON_COLOR,
    paddingVertical: 14,
    borderRadius: 10,
    alignItems: 'center',
    marginTop: 16,
  },

  submitButtonDisabled: {
    backgroundColor: '#9ca3af',
  },

  submitButtonText: {
    color: 'white',
    fontWeight: '700',
    fontSize: 16,
  },

  separator: {
    height: 1,
    backgroundColor: '#e6e6e6',
    marginVertical: 16,
  },
});

export default pollStyles;
