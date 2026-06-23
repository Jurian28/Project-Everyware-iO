import React, { useEffect, useState, useMemo } from 'react';
import { View, Text, ActivityIndicator, Alert, Pressable, StyleSheet } from 'react-native';
import { useRoute, useNavigation, type RouteProp, type NavigationProp, type ParamListBase } from '@react-navigation/native';
import AppLayout from '../../layouts/AppLayout';
import scheduleStyles from '../../styles/scheduleStyles';
import ScheduleTimeline from '../../components/event/ScheduleTimeline';
import TagFilterDropdown from '../../components/event/TagFilterDropdown';
import { SessionService, SessionDTO } from '../../services/SessionService';
import Colors from '../../enums/colors';
import { useEventContext } from '../../context/EventContext';

type EventScheduleParams = {
  eventId: number | string;
  eventTitle?: string;
  eventColor?: string;
  eventAccentColor?: string;
};

export default function EventSchedule() {
  const route = useRoute<RouteProp<{ EventSchedule: EventScheduleParams }, 'EventSchedule'>>();
  const navigation = useNavigation<NavigationProp<ParamListBase>>();
  const { eventId, eventTitle, eventColor, eventAccentColor } = route.params;

  const { setCurrentEventName } = useEventContext();

  const [loading, setLoading] = useState(true);
  const [sessions, setSessions] = useState<SessionDTO[]>([]);
  const [currentDateIndex, setCurrentDateIndex] = useState(0);
  const [selectedTag, setSelectedTag] = useState<string | null>(null);
  const [personal, setPersonal] = useState(false);

  useEffect(() => {
    if (eventTitle) setCurrentEventName(eventTitle);
    return () => setCurrentEventName(null);
  }, [eventTitle]);

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
    let filtered = sessions.filter(s => {
      const dateMatch = new Date(s.startTime).toDateString() === activeDateString;
      const tagMatch = selectedTag === null || s.tags.some(t => t.title === selectedTag);
      return dateMatch && tagMatch;
    });

    if (personal) {
      filtered = filtered.filter(s => s.plenary || s.isEnrolled);
    }

    return filtered;
  }, [sessions, activeDateString, selectedTag, personal]);

  const handlePrevDay = () => {
    if (currentDateIndex > 0) setCurrentDateIndex(currentDateIndex - 1);
  };
  const handleNextDay = () => {
    if (currentDateIndex < uniqueDays.length - 1) setCurrentDateIndex(currentDateIndex + 1);
  };

  const { startHour, endHour } = useMemo(() => {
    if (activeDateSessions.length === 0) return { startHour: 8, endHour: 18 };
    let minT = 24, maxT = 0;
    activeDateSessions.forEach(s => {
      const hStart = new Date(s.startTime).getHours();
      const hEnd = new Date(s.endTime).getHours() + (new Date(s.endTime).getMinutes() > 0 ? 1 : 0);
      if (hStart < minT) minT = hStart;
      if (hEnd > maxT) maxT = hEnd;
    });
    return {
      startHour: Math.max(0, minT - 1),
      endHour: Math.min(23, maxT + 1),
    };
  }, [activeDateSessions]);

  const prevArrowColor = currentDateIndex === 0 ? '#CBD5E1' : Colors.DEFAULT_BUTTON_COLOR;
  const nextArrowColor = currentDateIndex === uniqueDays.length - 1 ? '#CBD5E1' : Colors.DEFAULT_BUTTON_COLOR;

  return (
    <AppLayout>
      <View style={scheduleStyles.container}>
        <View style={scheduleStyles.header}>
          <Pressable onPress={() => navigation.goBack()} style={scheduleStyles.navButton}>
            <Text style={styles.backArrow}>&larr;</Text>
          </Pressable>
          <View style={styles.headerCenter}>
            <Text style={scheduleStyles.headerTitle}>{eventTitle || 'Event Schedule'}</Text>
            {uniqueDays.length > 0 && (
              <View style={styles.dayNavRow}>
                {uniqueDays.length > 1 && (
                  <Pressable onPress={handlePrevDay} disabled={currentDateIndex === 0} style={styles.dayNavButton}>
                    <Text style={[styles.dayNavArrow, { color: prevArrowColor }]}>&larr;</Text>
                  </Pressable>
                )}
                <Text style={scheduleStyles.monthTitle}>
                  {new Date(activeDateString).toLocaleDateString(undefined, {
                    weekday: 'long', month: 'short', day: 'numeric'
                  })}
                </Text>
                {uniqueDays.length > 1 && (
                  <Pressable onPress={handleNextDay} disabled={currentDateIndex === uniqueDays.length - 1} style={styles.dayNavButton}>
                    <Text style={[styles.dayNavArrow, { color: nextArrowColor }]}>&rarr;</Text>
                  </Pressable>
                )}
              </View>
            )}
          </View>
          <Pressable
            onPress={() => setPersonal(prev => !prev)}
            style={scheduleStyles.navButton}
          >
            <Text style={styles.personalToggle}>
              {personal ? 'All' : 'Personal'}
            </Text>
          </Pressable>
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
          <View style={styles.centered}>
            <ActivityIndicator size="large" color={Colors.DEFAULT_BUTTON_COLOR} />
          </View>
        ) : sessions.length === 0 ? (
          <View style={styles.centered}>
            <Text style={styles.emptyText}>No sessions available</Text>
          </View>
        ) : activeDateSessions.length === 0 ? (
          <View style={styles.centered}>
            <Text style={styles.emptyText}>No sessions match the selected tag</Text>
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

const styles = StyleSheet.create({
  backArrow: {
    fontSize: 24,
    color: '#1E293B',
  },
  headerCenter: {
    alignItems: 'center',
  },
  dayNavRow: {
    flexDirection: 'row',
    alignItems: 'center',
    marginTop: 4,
  },
  dayNavButton: {
    paddingHorizontal: 10,
  },
  dayNavArrow: {
    fontSize: 16,
  },
  personalToggle: {
    fontSize: 16,
    color: '#1E293B',
  },
  centered: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
  },
  emptyText: {
    color: '#64748B',
  },
});
