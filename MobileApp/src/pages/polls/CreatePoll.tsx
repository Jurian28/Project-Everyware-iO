import { useNavigation, useRoute } from '@react-navigation/native';
import React, { useState } from 'react';
import {
  Alert,
  ScrollView,
  Text,
  TextInput,
  TouchableOpacity,
  View,
} from 'react-native';

import AppLayout from '../../layouts/AppLayout';
import AuthService from '../../services/AuthService';
import { PollService } from '../../services/PollService';
import pollStyles from '../../styles/pollStyles';

export default function CreatePoll() {
  const route = useRoute<any>();
  const navigation = useNavigation<any>();

  const { sessionId } = route.params;

  const [userRoles, setUserRoles] = useState<string[]>([]);
  const [rolesLoaded, setRolesLoaded] = useState(false);

  React.useEffect(() => {
    AuthService.getUserRoles().then(roles => {
      setUserRoles(roles);
      setRolesLoaded(true);
    });
  }, []);

  const hasPollAccess = userRoles.some(
    r => r === 'Speaker' || r === 'Admin',
  );

  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [answers, setAnswers] = useState<string[]>(['', '']);
  const [submitting, setSubmitting] = useState(false);

  const handleAddAnswer = () => {
    setAnswers([...answers, '']);
  };

  const handleRemoveAnswer = (index: number) => {
    if (answers.length <= 1) return;
    setAnswers(answers.filter((_, i) => i !== index));
  };

  const handleAnswerChange = (index: number, value: string) => {
    const updated = [...answers];
    updated[index] = value;
    setAnswers(updated);
  };

  const handleSubmit = async () => {
    if (!title.trim()) {
      Alert.alert('Validation', 'Please enter a poll title.');
      return;
    }

    const nonEmptyAnswers = answers.filter(a => a.trim().length > 0);
    if (nonEmptyAnswers.length < 1) {
      Alert.alert('Validation', 'Please add at least one answer.');
      return;
    }

    try {
      setSubmitting(true);

      const res = await PollService.createPoll(
        title.trim(),
        description.trim() || undefined,
        nonEmptyAnswers.map(a => a.trim()),
        sessionId,
      );

      if (res.success) {
        navigation.goBack();
      } else {
        Alert.alert('Error', res.message ?? 'Failed to create poll');
      }
    } catch (e: any) {
      Alert.alert('Error', e.message ?? 'Failed to create poll');
    } finally {
      setSubmitting(false);
    }
  };

  if (!rolesLoaded) return null;
  if (!hasPollAccess)
    return (
      <AppLayout>
        <View style={pollStyles.center}>
          <Text style={{ color: 'red', fontSize: 16, textAlign: 'center' }}>
            You don't have permission to create polls. Only Speakers and Admins can access this feature.
          </Text>
          <TouchableOpacity
            onPress={() => navigation.goBack()}
            style={[pollStyles.button, { marginTop: 20 }]}
          >
            <Text style={pollStyles.buttonText}>Go Back</Text>
          </TouchableOpacity>
        </View>
      </AppLayout>
    );

  return (
    <AppLayout>
      <ScrollView contentContainerStyle={pollStyles.container}>
        <TouchableOpacity
          onPress={() => navigation.goBack()}
          style={pollStyles.backButton}
        >
          <Text style={pollStyles.backText}>← Back to Polls</Text>
        </TouchableOpacity>

        <View style={pollStyles.header}>
          <Text style={pollStyles.headerTitle}>Create Poll</Text>
        </View>

        <View style={pollStyles.formGroup}>
          <Text style={pollStyles.label}>Title</Text>
          <TextInput
            style={pollStyles.input}
            value={title}
            onChangeText={setTitle}
            placeholder="Enter poll title"
          />
        </View>

        <View style={pollStyles.formGroup}>
          <Text style={pollStyles.label}>Description (optional)</Text>
          <TextInput
            style={[pollStyles.input, pollStyles.textArea]}
            value={description}
            onChangeText={setDescription}
            placeholder="Enter poll description"
            multiline
          />
        </View>

        <View style={pollStyles.formGroup}>
          <Text style={pollStyles.label}>Answers</Text>
          {answers.map((answer, index) => (
            <View key={index} style={pollStyles.answerInputRow}>
              <TextInput
                style={pollStyles.answerInput}
                value={answer}
                onChangeText={value => handleAnswerChange(index, value)}
                placeholder={`Answer ${index + 1}`}
              />
              {answers.length > 1 && (
                <TouchableOpacity
                  style={pollStyles.removeButton}
                  onPress={() => handleRemoveAnswer(index)}
                >
                  <Text style={pollStyles.removeButtonText}>Remove</Text>
                </TouchableOpacity>
              )}
            </View>
          ))}
          <TouchableOpacity
            style={pollStyles.addButton}
            onPress={handleAddAnswer}
          >
            <Text style={pollStyles.addButtonText}>+ Add Answer</Text>
          </TouchableOpacity>
        </View>

        <TouchableOpacity
          style={[
            pollStyles.submitButton,
            submitting && pollStyles.submitButtonDisabled,
          ]}
          onPress={handleSubmit}
          disabled={submitting}
        >
          <Text style={pollStyles.submitButtonText}>
            {submitting ? 'Creating...' : 'Create Poll'}
          </Text>
        </TouchableOpacity>
      </ScrollView>
    </AppLayout>
  );
}
