import { useFocusEffect, useNavigation, useRoute } from '@react-navigation/native';
import React, { useCallback, useState } from 'react';
import {
  ActivityIndicator,
  ScrollView,
  Text,
  TouchableOpacity,
  View,
} from 'react-native';

import AppLayout from '../../layouts/AppLayout';
import AuthService from '../../services/AuthService';
import { PollDTO, PollService } from '../../services/PollService';
import pollStyles from '../../styles/pollStyles';

function Loading() {
  return (
    <View style={pollStyles.center}>
      <ActivityIndicator size="large" />
      <Text style={{ marginTop: 10 }}>Loading polls...</Text>
    </View>
  );
}

function ErrorState({ message }: { message: string }) {
  return (
    <View style={pollStyles.center}>
      <Text style={{ color: 'red' }}>{message}</Text>
    </View>
  );
}

function PollCard({
  poll,
  onDelete,
  onClose,
  showActions,
}: {
  poll: PollDTO;
  onDelete: () => void;
  onClose: () => void;
  showActions: boolean;
}) {
  return (
    <View style={pollStyles.pollCard}>
      <View style={pollStyles.pollMeta}>
        <Text style={pollStyles.pollTitle}>{poll.title}</Text>
        <View
          style={[
            pollStyles.statusBadge,
            poll.isClosed ? pollStyles.statusClosed : pollStyles.statusOpen,
          ]}
        >
          <Text
            style={[
              pollStyles.statusBadge,
              { color: poll.isClosed ? '#374151' : '#065f46' },
            ]}
          >
            {poll.isClosed ? 'Closed' : 'Open'}
          </Text>
        </View>
      </View>

      {poll.description && (
        <Text style={pollStyles.pollDescription}>{poll.description}</Text>
      )}

      <View style={pollStyles.answersList}>
        {poll.answers.map((answer, index) => (
          <View key={answer.idPollAnswer} style={pollStyles.answerItem}>
            <Text style={pollStyles.answerText}>
              {index + 1}. {answer.text}
            </Text>
          </View>
        ))}
      </View>

      {showActions && (
        <View style={pollStyles.actionRow}>
          {!poll.isClosed && (
            <TouchableOpacity
              style={[pollStyles.button, pollStyles.closeButton]}
              onPress={onClose}
            >
              <Text style={pollStyles.buttonText}>Close</Text>
            </TouchableOpacity>
          )}
          <TouchableOpacity
            style={[pollStyles.button, pollStyles.deleteButton]}
            onPress={onDelete}
          >
            <Text style={pollStyles.buttonText}>Delete</Text>
          </TouchableOpacity>
        </View>
      )}
    </View>
  );
}

export default function ManagePolls() {
  const route = useRoute<any>();
  const navigation = useNavigation<any>();

  const { sessionId } = route.params;

  const [polls, setPolls] = useState<PollDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
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

  const loadPolls = async () => {
    try {
      setLoading(true);
      setError(null);

      const res = await PollService.getSessionPolls(sessionId);

      if (res.success && res.polls) {
        setPolls(res.polls);
      } else {
        setError(res.message ?? 'Failed to load polls');
      }
    } catch (e: any) {
      setError(e?.message ?? 'Something went wrong');
    } finally {
      setLoading(false);
    }
  };

  useFocusEffect(
    useCallback(() => {
      loadPolls();
    }, [sessionId]),
  );

  const handleDelete = async (poll: PollDTO) => {
    try {
      const res = await PollService.deletePoll(poll.idPoll);
      if (res.success) {
        await loadPolls();
      } else {
        console.error('Delete failed:', res.message);
      }
    } catch (e: any) {
      console.error('Delete error:', e?.message);
    }
  };

  const handleClose = async (poll: PollDTO) => {
    try {
      const res = await PollService.closePoll(poll.idPoll);
      if (res.success) {
        await loadPolls();
      } else {
        alert(res.message ?? 'Failed to close poll');
      }
    } catch (e: any) {
      alert(e.message ?? 'Failed to close poll');
    }
  };

  if (!rolesLoaded || loading) return <Loading />;
  if (error) return <ErrorState message={error} />;

  return (
    <AppLayout>
      <ScrollView
        contentContainerStyle={pollStyles.container}
        keyboardShouldPersistTaps="handled"
      >
        <TouchableOpacity
          onPress={() => navigation.goBack()}
          style={pollStyles.backButton}
        >
          <Text style={pollStyles.backText}>← Back to Session</Text>
        </TouchableOpacity>

        <View style={pollStyles.header}>
          <Text style={pollStyles.headerTitle}>
            {hasPollAccess ? 'Manage Polls' : 'Polls'}
          </Text>
          {hasPollAccess && (
            <TouchableOpacity
              style={pollStyles.createButton}
              onPress={() =>
                navigation.navigate('CreatePoll', { sessionId })
              }
            >
              <Text style={pollStyles.createButtonText}>+ Create Poll</Text>
            </TouchableOpacity>
          )}
        </View>

        {polls.length === 0 ? (
          <Text style={pollStyles.emptyText}>
            {hasPollAccess
              ? 'No polls yet. Create one to get started.'
              : 'No polls available.'}
          </Text>
        ) : (
          polls.map(poll => (
            <PollCard
              key={poll.idPoll}
              poll={poll}
              onDelete={() => handleDelete(poll)}
              onClose={() => handleClose(poll)}
              showActions={hasPollAccess}
            />
          ))
        )}
      </ScrollView>
    </AppLayout>
  );
}
