import React, { useEffect, useState, useMemo } from 'react';
import { View, Text, ScrollView } from 'react-native';
import scheduleStyles, { MINUTE_HEIGHT, TIMELINE_OFFSET_LEFT } from '../../styles/scheduleStyles';
import { SessionDTO } from '../../services/SessionService';

interface ScheduleTimelineProps {
  sessions: SessionDTO[];
  currentDate: Date;
  startHour?: number;
  endHour?: number;
  eventColor?: string;
  eventAccentColor?: string;
}

const getContrastTextColor = (hexColor: string | undefined) => {
  if (!hexColor) return '#0F172A';
  let hex = hexColor.replace('#', '');
  if (hex.length === 3) {
    hex = hex.split('').map(char => char + char).join('');
  }
  if (hex.length !== 6) return '#0F172A';
  const r = parseInt(hex.slice(0, 2), 16);
  const g = parseInt(hex.slice(2, 4), 16);
  const b = parseInt(hex.slice(4, 6), 16);
  const luminance = (0.299 * r + 0.587 * g + 0.114 * b);
  return luminance > 160 ? '#0F172A' : '#FFFFFF';
};

export default function ScheduleTimeline({ sessions, currentDate, startHour = 8, endHour = 18, eventColor, eventAccentColor }: ScheduleTimelineProps) {
  const [now, setNow] = useState(new Date());

  useEffect(() => {
    const interval = setInterval(() => setNow(new Date()), 60000);
    return () => clearInterval(interval);
  }, []);

  const isToday = (d1: Date, d2: Date) => 
    d1.getDate() === d2.getDate() && d1.getMonth() === d2.getMonth() && d1.getFullYear() === d2.getFullYear();

  const layoutedSessions = useMemo(() => {
    const sorted = [...sessions].sort((a, b) => new Date(a.startTime).getTime() - new Date(b.startTime).getTime());
    
    const groups: SessionDTO[][] = [];
    let currentGroup: SessionDTO[] = [];
    let groupMaxEnd = 0;

    sorted.forEach((session) => {
      const start = new Date(session.startTime).getTime();
      const end = new Date(session.endTime).getTime();
      
      if (currentGroup.length === 0) {
        currentGroup.push(session);
        groupMaxEnd = end;
      } else {
        if (start < groupMaxEnd) {
          currentGroup.push(session);
          groupMaxEnd = Math.max(groupMaxEnd, end);
        } else {
          groups.push(currentGroup);
          currentGroup = [session];
          groupMaxEnd = end;
        }
      }
    });
    if (currentGroup.length > 0) {
      groups.push(currentGroup);
    }

    const layout = sorted.map(s => {
      const startD = new Date(s.startTime);
      const endD = new Date(s.endTime);
      
      const topMinutes = (startD.getHours() - startHour) * 60 + startD.getMinutes();
      const top = topMinutes * MINUTE_HEIGHT;
      
      const durationMinutes = (endD.getTime() - startD.getTime()) / 60000;
      const height = Math.max(durationMinutes * MINUTE_HEIGHT, 20); // minimum height

      return { ...s, top, height, left: 0, width: 100 };
    });

    groups.forEach((group) => {
      let columns: SessionDTO[][] = [];
      group.forEach((session) => {
        const start = new Date(session.startTime).getTime();
        let placed = false;
        for (let i = 0; i < columns.length; i++) {
          const col = columns[i];
          const lastInCol = col[col.length - 1];
          const lastEnd = new Date(lastInCol.endTime).getTime();
          if (start >= lastEnd) {
            col.push(session);
            placed = true;
            break;
          }
        }
        if (!placed) {
          columns.push([session]);
        }
      });

      const colCount = columns.length;
      columns.forEach((col, colIndex) => {
        col.forEach(sess => {
          const lSess = layout.find(l => l.sessionId === sess.sessionId);
          if (lSess) {
            lSess.width = 100 / colCount;
            lSess.left = colIndex * (100 / colCount);
          }
        });
      });
    });

    return layout;
  }, [sessions, startHour]);

  const renderHours = () => {
    const hours = [];
    for (let h = startHour; h <= endHour; h++) {
      hours.push(
        <View key={h} style={scheduleStyles.hourRow}>
          <View style={scheduleStyles.timeLabelContainer}>
            <Text style={scheduleStyles.timeLabel}>{`${h.toString().padStart(2, '0')}:00`}</Text>
          </View>
          <View style={scheduleStyles.timeRowLine} />
        </View>
      );
    }
    return hours;
  };

  const currentTopMinutes = (now.getHours() - startHour) * 60 + now.getMinutes();
  const currentTop = currentTopMinutes * MINUTE_HEIGHT;
  const showCurrentTime = isToday(now, currentDate) && now.getHours() >= startHour && now.getHours() <= endHour;

  return (
    <ScrollView style={scheduleStyles.scrollView} contentContainerStyle={{ paddingVertical: 10 }}>
      {renderHours()}

      {showCurrentTime && (
        <View style={[scheduleStyles.currentTimeLineContainer, { top: currentTop + 10 }]}> 
          <View style={scheduleStyles.currentTimeCircle} />
          <View style={scheduleStyles.currentTimeLine} />
        </View>
      )}

      <View style={[scheduleStyles.cardsContainer, { position: 'absolute', top: 10, left: TIMELINE_OFFSET_LEFT, right: 0, bottom: 0 }]}>
        {layoutedSessions.map((session) => {
          const textColor = getContrastTextColor(eventColor);
          const subtitleColor = textColor === '#FFFFFF' ? '#E2E8F0' : '#475569';
          return (
            <View
              key={session.sessionId}
              style={[
                scheduleStyles.sessionCard,
                {
                  top: session.top,
                  height: session.height,
                  left: `${session.left}%`,
                  width: `${session.width}%`,
                  backgroundColor: eventColor || '#FFFFFF',
                  borderLeftColor: eventAccentColor || '#3B82F6',
                }
              ]}
            >
              <View style={{ flex: 1, overflow: 'hidden' }}>
                <Text numberOfLines={1} style={[scheduleStyles.sessionTitle, { color: textColor }]}>{session.title}</Text>
                {session.height > 40 && <Text numberOfLines={1} style={[scheduleStyles.sessionSubtitle, { color: subtitleColor }]}>{session.speakerName}</Text>}
                {session.height > 60 && <Text numberOfLines={1} style={[scheduleStyles.sessionSubtitle, { color: subtitleColor }]}>{session.room.roomLabel}</Text>}
              </View>
              
              {session.height > 50 && (
                <View style={scheduleStyles.tagContainer}>
                  {session.tags && session.tags.map(t => (
                    <View key={t.idTag} style={[scheduleStyles.tagBadge, { borderColor: t.colorHex, backgroundColor: `${t.colorHex}20` }]}>
                      <Text style={[scheduleStyles.tagText, { color: t.colorHex }]}>{t.title}</Text>
                    </View>
                  ))}
                </View>
              )}
            </View>
          );
        })}
      </View>
    </ScrollView>
  );
}
