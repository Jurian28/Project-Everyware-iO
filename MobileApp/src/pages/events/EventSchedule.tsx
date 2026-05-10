import React, { useEffect, useState, useMemo } from 'react';
import { View, Text, ActivityIndicator, Alert, Pressable } from 'react-native';
import { useRoute, useNavigation } from '@react-navigation/native';
import AppLayout from '../../layouts/AppLayout';
import scheduleStyles from '../../styles/scheduleStyles';
import ScheduleTimeline from '../../components/event/ScheduleTimeline';
import { SessionService, SessionDTO } from '../../services/SessionService';

export default function EventSchedule() {
  const route = useRoute<any>();
  const navigation = useNavigation<any>();
  const { eventId, eventTitle, eventColor } = route.params as { eventId: number | string; eventTitle?: string; eventColor?: string };

  const [loading, setLoading] = useState(true);
  const [sessions, setSessions] = useState<SessionDTO[]>([]);
  const [currentDateIndex, setCurrentDateIndex] = useState(0);

  useEffect(() => {
    const loadData = async () => {
      setLoading(true);
      const res = await SessionService.fetchEventSessions(eventId);
      if (res.success && res.sessions) {
        setSessions(res.sessions);
      } else {
        Alert.alert('Error', 'Failed to load sessions');
      }
      setLoading(false);
    };
    loadData();
  }, [eventId]);

  const uniqueDays = useMemo(() => {
    const days = new Set<string>();
    sessions.forEach(s => {
      const d = new Date(s.startTime);
      days.add(d.toDateString());
    });
    const sorted = Array.from(days).sort((a, b) => new Date(a).getTime() - new Date(b).getTime());
    return sorted;
  }, [sessions]);

  const activeDateString = uniqueDays.length > 0 ? uniqueDays[currentDateIndex] : new Date().toDateString();
  const activeDateSessions = useMemo(() => {
    return sessions.filter(s => new Date(s.startTime).toDateString() === activeDateString);
  }, [sessions, activeDateString]);

  const handlePrevDay = () => {
    if (currentDateIndex > 0) setCurrentDateIndex(currentDateIndex - 1);
  };
  const handleNextDay = () => {
    if (currentDateIndex < uniqueDays.length - 1) setCurrentDateIndex(currentDateIndex + 1);
  };

  const startHour = 0;
  const endHour = 23;

  return (
    <AppLayout>
      <View style={scheduleStyles.container}>
        <View style={scheduleStyles.header}>
          <Pressable onPress={() => navigation.goBack()} style={scheduleStyles.navButton}>
            <Text style={{ fontSize: 24, color: '#1E293B' }}>&larr;</Text>
          </Pressable>
          <View style={{ alignItems: 'center' }}>
            <Text style={scheduleStyles.headerTitle}>{eventTitle || 'Event Schedule'}</Text>
            {uniqueDays.length > 0 && (
              <View style={{ flexDirection: 'row', alignItems: 'center', marginTop: 4 }}>
                <Pressable onPress={handlePrevDay} disabled={currentDateIndex === 0} style={{ paddingHorizontal: 10 }}>
                  <Text style={{ color: currentDateIndex === 0 ? '#CBD5E1' : '#3B82F6', fontSize: 16 }}>&larr;</Text>
                </Pressable>
                <Text style={scheduleStyles.monthTitle}>
                  {new Date(activeDateString).toLocaleDateString(undefined, { weekday: 'long', month: 'short', day: 'numeric' })}
                </Text>
                <Pressable onPress={handleNextDay} disabled={currentDateIndex === uniqueDays.length - 1} style={{ paddingHorizontal: 10 }}>
                  <Text style={{ color: currentDateIndex === uniqueDays.length - 1 ? '#CBD5E1' : '#3B82F6', fontSize: 16 }}>&rarr;</Text>
                </Pressable>
              </View>
            )}
          </View>
          <View style={{ width: 40 }} />
        </View>

        {loading ? (
          <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
            <ActivityIndicator size="large" color="#3B82F6" />
          </View>
        ) : sessions.length === 0 ? (
          <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
            <Text style={{ color: '#64748B' }}>No sessions available</Text>
          </View>
        ) : (
          <ScheduleTimeline 
            sessions={activeDateSessions} 
            currentDate={new Date(activeDateString)}
            startHour={startHour}
            endHour={endHour}
            eventColor={eventColor}
          />
        )}
      </View>
    </AppLayout>
  );
}
