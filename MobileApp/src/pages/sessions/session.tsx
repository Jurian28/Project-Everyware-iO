import { useNavigation, useRoute } from '@react-navigation/native';
import React, { useEffect, useState } from 'react';
import {
    ActivityIndicator,
    ScrollView,
    Text,
    TouchableOpacity,
    View,
} from 'react-native';

import EnrollConflictModal from '../../components/sessions/enrollConflictModal';
import { Session, SessionService, Tag } from '../../services/SessionService';

import sessionStyles from '../../styles/sessionStyles';
import { formatTimeRange } from '../../utils/dates';

type ConflictSession = {
    idSession: number;
    title: string;
    startTime: string;
    endTime: string;
};

function TagItem({ tag }: { tag: Tag }) {
    return (
        <View style={[sessionStyles.tag, { borderColor: tag.colorHex }]}>
            <Text style={[sessionStyles.tagText, { color: tag.colorHex }]}>
                {tag.title}
            </Text>
        </View>
    );
}

function Section({
    title,
    children,
}: {
    title: string;
    children: React.ReactNode;
}) {
    return (
        <>
            <Separator />
            <Text style={sessionStyles.sectionTitle}>{title}</Text>
            {children}
        </>
    );
}

function Separator() {
    return <View style={sessionStyles.separator} />;
}

function Loading() {
    return (
        <View style={sessionStyles.center}>
            <ActivityIndicator size="large" />
            <Text style={{ marginTop: 10 }}>Loading session...</Text>
        </View>
    );
}

function ErrorState({ message }: { message: string }) {
    return (
        <View style={sessionStyles.center}>
            <Text style={{ color: 'red' }}>{message}</Text>
        </View>
    );
}

function PlacesLeft({
    placesLeft,
    isEnrolled,
    inQueue,
    onEnroll,
    onWithdraw,
    loading,
}: {
    placesLeft: number;
    isEnrolled: boolean;
    inQueue: boolean;
    onEnroll?: () => void;
    onWithdraw?: () => void;
    loading?: boolean;
}) {
    return (
        <View style={sessionStyles.enrollRow}>
            <Text style={sessionStyles.spotsInlineText}>
                {placesLeft} spaces left
                {inQueue && ' - You are in the waiting list'}
            </Text>

            <TouchableOpacity
                disabled={loading}
                onPress={isEnrolled ? onWithdraw : onEnroll}
                style={[
                    sessionStyles.actionButton,
                    isEnrolled
                        ? sessionStyles.withdrawButton
                        : sessionStyles.enrollButton,
                    loading && sessionStyles.disabledButton,
                ]}
            >
                <Text style={sessionStyles.actionText}>
                    {loading
                        ? 'Loading...'
                        : isEnrolled
                        ? 'Withdraw from session'
                        : 'Enroll for session'}
                </Text>
            </TouchableOpacity>
        </View>
    );
}

export default function SessionViewPage() {
    const route = useRoute<any>();
    const navigation = useNavigation<any>();

    const { sessionId, eventMainColorHex } = route.params;

    const [session, setSession] = useState<Session | null>(null);

    const [loading, setLoading] = useState(true);

    const [error, setError] = useState<string | null>(null);

    const [actionLoading, setActionLoading] = useState(false);

    const [conflictVisible, setConflictVisible] = useState(false);

    const [conflictSession, setConflictSession] =
        useState<ConflictSession | null>(null);

    const loadSession = async () => {
        try {
            setLoading(true);
            setError(null);

            const data = await SessionService.getSession(sessionId);

            setSession(data);
        } catch (e: any) {
            setError(e?.message ?? 'Something went wrong');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadSession();
    }, [sessionId]);

    const handleEnroll = async () => {
        if (!session) return;

        try {
            setActionLoading(true);

            const res = await SessionService.enroll(session.sessionId);

            if (res?.isConflict === true) {
                setConflictSession(res.conflictingSessions[0]);
                setConflictVisible(true);
                return;
            }

            await loadSession();
        } catch (e: any) {
            alert(e.message ?? 'Enroll failed');
        } finally {
            setActionLoading(false);
        }
    };

    const handleWithdraw = async () => {
        if (!session) return;

        try {
            setActionLoading(true);

            await SessionService.withdraw(session.sessionId);

            await loadSession();
        } catch (e: any) {
            alert(e.message ?? 'Withdraw failed');
        } finally {
            setActionLoading(false);
        }
    };

    const confirmOverride = async () => {
        if (!session || !conflictSession) return;

        try {
            setActionLoading(true);

            await SessionService.enroll(session.sessionId, true);

            await loadSession();
        } catch (e: any) {
            alert(e.message ?? 'Failed to switch sessions');
        } finally {
            setActionLoading(false);
            setConflictVisible(false);
        }
    };

    if (loading) return <Loading />;

    if (error) return <ErrorState message={error} />;

    if (!session) return <ErrorState message="Session not found" />;

    return (
        <ScrollView contentContainerStyle={sessionStyles.container}>
            <View style={sessionStyles.detailsHeader}>
                <TouchableOpacity
                    onPress={() => navigation.goBack()}
                    style={sessionStyles.backButton}
                >
                    <Text style={sessionStyles.backText}>← Back</Text>
                </TouchableOpacity>

                <TouchableOpacity 
                    style={sessionStyles.qrCodeButton} 
                    onPress={() => navigation.navigate('SessionQRCode', { eventMainColorHex, session: session })}
                >
                    <Text style={sessionStyles.qrCodeButtonText}>Attendance QR Code</Text>
                </TouchableOpacity>
            </View>

            <Text style={sessionStyles.title}>{session.title}</Text>

            <Text style={sessionStyles.time}>
                {formatTimeRange(session.startTime, session.endTime)}
            </Text>

            {session.room?.roomLabel && (
                <Text style={sessionStyles.location}>
                    {session.room.roomLabel}
                </Text>
            )}

            {!!session.tags?.length && (
                <View style={sessionStyles.tagRow}>
                    {session.tags.map(tag => (
                        <TagItem key={tag.idTag} tag={tag} />
                    ))}
                </View>
            )}

            <PlacesLeft
                placesLeft={session.placesLeft}
                isEnrolled={session.isEnrolled ?? false}
                inQueue={session.inQueue ?? false}
                onEnroll={handleEnroll}
                onWithdraw={handleWithdraw}
                loading={actionLoading}
            />

            <Section title="Speakers">
                <Text style={sessionStyles.speakerName}>
                    {session.speakerName ?? 'TBA'}
                </Text>
            </Section>

            <Section title="About">
                <Text style={sessionStyles.aboutText}>
                    {session.description ?? 'No description available yet.'}
                </Text>
            </Section>

            <EnrollConflictModal
                visible={conflictVisible}
                conflictSession={conflictSession}
                onCancel={() => setConflictVisible(false)}
                onConfirm={confirmOverride}
            />
        </ScrollView>
    );
}
