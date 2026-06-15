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

function ResultsBar({
  voteCount,
  totalVotes,
  isSelected,
  answerText,
}: {
  voteCount: number;
  totalVotes: number;
  isSelected: boolean;
  answerText: string;
}) {
  const percentage = totalVotes > 0 ? (voteCount / totalVotes) * 100 : 0;

  return (
    <View
      style={[
        pollStyles.answerItem,
        pollStyles.answerItemResult,
        isSelected && pollStyles.answerItemSelected,
      ]}
    >
      <View style={pollStyles.resultBarContainer}>
        <View
          style={[
            pollStyles.resultBarFill,
            { width: `${Math.max(percentage, 2)}%` },
            isSelected && pollStyles.resultBarFillSelected,
          ]}
        />
      </View>
      <View
        style={[
          pollStyles.resultContent,
          isSelected && { paddingRight: 24 },
        ]}
      >
        <Text style={pollStyles.answerText}>{answerText}</Text>
        <Text style={pollStyles.voteCountText}>
          {Math.round(percentage)}% ({voteCount})
        </Text>
      </View>
      {isSelected && <Text style={pollStyles.checkMark}>✓</Text>}
    </View>
  );
}

function VoteOption({
  answerText,
  answerId,
  disabled,
  onVote,
}: {
  answerText: string;
  answerId: number;
  disabled: boolean;
  onVote: (answerId: number) => void;
}) {
  return (
    <TouchableOpacity
      style={[pollStyles.answerItem, pollStyles.answerItemVotable]}
      onPress={() => onVote(answerId)}
      disabled={disabled}
    >
      <Text style={pollStyles.answerText}>{answerText}</Text>
    </TouchableOpacity>
  );
}

function PollCard({
  poll,
  onVote,
  onDelete,
  onClose,
  showActions,
  canVote,
  isVoting,
}: {
  poll: PollDTO;
  onVote: (answerId: number) => void;
  onDelete: () => void;
  onClose: () => void;
  showActions: boolean;
  canVote: boolean;
  isVoting: boolean;
}) {
  const totalVotes = poll.answers.reduce((sum, a) => sum + a.voteCount, 0);

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
            style={{
              color: poll.isClosed ? '#374151' : '#065f46',
              fontSize: 12,
              fontWeight: '600',
            }}
          >
            {poll.isClosed ? 'Closed' : 'Open'}
          </Text>
        </View>
      </View>

      {poll.description && (
        <Text style={pollStyles.pollDescription}>{poll.description}</Text>
      )}

      {totalVotes > 0 && (
        <Text style={pollStyles.totalVotesText}>
          {totalVotes} vote{totalVotes !== 1 ? 's' : ''}
        </Text>
      )}

      <View style={pollStyles.answersList}>
        {canVote && !poll.isClosed && !poll.hasVoted
          ? poll.answers.map(answer => (
              <VoteOption
                key={answer.idPollAnswer}
                answerText={answer.text}
                answerId={answer.idPollAnswer}
                disabled={isVoting}
                onVote={onVote}
              />
            ))
          : poll.answers.map(answer => (
              <ResultsBar
                key={answer.idPollAnswer}
                answerText={answer.text}
                voteCount={answer.voteCount}
                totalVotes={totalVotes}
                isSelected={answer.idPollAnswer === poll.votedAnswerId}
              />
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
  const [votingPollId, setVotingPollId] = useState<number | null>(null);

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

  const handleVote = async (poll: PollDTO, answerId: number) => {
    try {
      setVotingPollId(poll.idPoll);
      const res = await PollService.vote(poll.idPoll, answerId);
      if (res.success && res.poll) {
        setPolls(prev =>
          prev.map(p => (p.idPoll === poll.idPoll ? res.poll! : p)),
        );
      } else {
        alert(res.message ?? 'Failed to vote');
      }
    } catch (e: any) {
      alert(e.message ?? 'Failed to vote');
    } finally {
      setVotingPollId(null);
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
              onVote={answerId => handleVote(poll, answerId)}
              onDelete={() => handleDelete(poll)}
              onClose={() => handleClose(poll)}
              showActions={hasPollAccess}
              canVote={!hasPollAccess}
              isVoting={votingPollId === poll.idPoll}
            />
          ))
        )}
      </ScrollView>
    </AppLayout>
  );
}
