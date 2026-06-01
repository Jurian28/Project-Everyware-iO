import React, { useEffect, useState, useMemo } from 'react';
import { View, Text, ActivityIndicator, Alert, Pressable } from 'react-native';
import { useRoute, useNavigation } from '@react-navigation/native';
import AppLayout from '../../layouts/AppLayout';
import scheduleStyles from '../../styles/scheduleStyles';
import ScheduleTimeline from '../../components/event/ScheduleTimeline';
import TagFilterDropdown from '../../components/event/TagFilterDropdown';
import { SessionService, SessionDTO } from '../../services/SessionService';

export default function EventSchedule() {
  const route = useRoute<any>();
  const navigation = useNavigation<any>();
  const { eventId, eventTitle, eventColor, eventAccentColor } = route.params as {
    eventId: number | string;
    eventTitle?: string;
    eventColor?: string;
    eventAccentColor?: string;
  };

  const [loading, setLoading] = useState(true);
  const [sessions, setSessions] = useState<SessionDTO[]>([]);
  const [currentDateIndex, setCurrentDateIndex] = useState(0);
  const [selectedTag, setSelectedTag] = useState<string | null>(null);

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
    return Array.from(days).sort((a, b) => new Date(a).getTime() - new Date(b).getTime());
  }, [sessions]);

  const uniqueTags = useMemo(() => {
    const visible = new Map<string, string>();
    sessions.forEach(s =>
      s.tags.forEach(t => {
        if (!visible.has(t.title)) visible.set(t.title, t.colorHex);
      })
    );
    return Array.from(visible.entries()).map(([title, colorHex]) => ({ title, colorHex }));
  }, [sessions]);

  const activeDateString = uniqueDays.length > 0 ? uniqueDays[currentDateIndex] : new Date().toDateString();
  const activeDateSessions = useMemo(() => {
    return sessions.filter(s => {
      const dateMatch = new Date(s.startTime).toDateString() === activeDateString;
      const tagMatch = selectedTag === null || s.tags.some(t => t.title === selectedTag);
      return dateMatch && tagMatch;
    });
  }, [sessions, activeDateString, selectedTag]);

  const handlePrevDay = () => {
    if (currentDateIndex > 0) setCurrentDateIndex(currentDateIndex - 1);
  };
  const handleNextDay = () => {
    if (currentDateIndex < uniqueDays.length - 1) setCurrentDateIndex(currentDateIndex + 1);
  };

  const { startHour, endHour } = useMemo(() => {
    if (activeDateSessions.length === 0) return { startHour: 8, endHour: 21 };
    let minT = 24, maxT = 0;
    activeDateSessions.forEach(s => {
      const hStart = new Date(s.startTime).getHours();
      const hEnd = new Date(s.endTime).getHours() + (new Date(s.endTime).getMinutes() > 0 ? 1 : 0);
      if (hStart < minT) minT = hStart;
      if (hEnd > maxT) maxT = hEnd;
    });
    return {
      startHour: Math.max(0, Math.min(8, minT)),
      endHour: Math.min(23, Math.max(21, maxT))
    };
  }, [activeDateSessions]);

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
                {uniqueDays.length > 1 && (
                  <Pressable onPress={handlePrevDay} disabled={currentDateIndex === 0} style={{ paddingHorizontal: 10 }}>
                    <Text style={{ color: currentDateIndex === 0 ? '#CBD5E1' : '#3B82F6', fontSize: 16 }}>&larr;</Text>
                  </Pressable>
                )}
                <Text style={scheduleStyles.monthTitle}>
                  {new Date(activeDateString).toLocaleDateString(undefined, {
                    weekday: 'long', month: 'short', day: 'numeric'
                  })}
                </Text>
                {uniqueDays.length > 1 && (
                  <Pressable onPress={handleNextDay} disabled={currentDateIndex === uniqueDays.length - 1} style={{ paddingHorizontal: 10 }}>
                    <Text style={{ color: currentDateIndex === uniqueDays.length - 1 ? '#CBD5E1' : '#3B82F6', fontSize: 16 }}>&rarr;</Text>
                  </Pressable>
                )}
              </View>
            )}
          </View>
          <View style={{ width: 40 }} />
        </View>

        {uniqueTags.length > 0 && (
          <TagFilterDropdown
            tags={uniqueTags}
            selectedTag={selectedTag}
            onSelect={setSelectedTag}
            accentColor={eventColor}
          />
        )}

        {loading ? (
          <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
            <ActivityIndicator size="large" color="#3B82F6" />
          </View>
        ) : sessions.length === 0 ? (
          <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
            <Text style={{ color: '#64748B' }}>No sessions available</Text>
          </View>
        ) : activeDateSessions.length === 0 ? (
          <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
            <Text style={{ color: '#64748B' }}>No sessions match the selected tag</Text>
          </View>
        ) : (
          <ScheduleTimeline
            sessions={activeDateSessions}
            currentDate={new Date(activeDateString)}
            startHour={startHour}
            endHour={endHour}
            eventColor={eventColor}
            eventAccentColor={eventAccentColor}
          />
        )}
      </View>
    </AppLayout>
  );
}
