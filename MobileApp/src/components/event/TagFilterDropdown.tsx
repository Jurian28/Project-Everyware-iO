import React, { useState } from 'react';
import { View, Text, Pressable, ScrollView } from 'react-native';
type TagOption = { title: string; colorHex: string };

type Props = {
    tags: TagOption[];
    selectedTag: string | null;
    onSelect: (tag: string | null) => void;
    accentColor?: string;
};

export default function TagFilterDropdown({ tags, selectedTag, onSelect, accentColor }: Props) {
    const [open, setOpen] = useState(false);

    return (
        <View style={{ zIndex: 10, paddingHorizontal: 16, paddingVertical: 8 }}>
            <Pressable
                onPress={() => setOpen(prev => !prev)}
                style={{
                    flexDirection: 'row',
                    alignItems: 'center',
                    alignSelf: 'flex-start',
                    borderWidth: 1,
                    borderColor: selectedTag ? (accentColor ?? '#3B82F6') : '#CBD5E1',
                    borderRadius: 8,
                    paddingHorizontal: 12,
                    paddingVertical: 6,
                    backgroundColor: '#FFFFFF',
                    gap: 6,
                }}
            >
                {selectedTag && (
                    <View style={{
                        width: 8, height: 8, borderRadius: 4,
                        backgroundColor: tags.find(t => t.title === selectedTag)?.colorHex ?? '#3B82F6',
                    }} />
                )}
                <Text style={{ color: '#1E293B', fontSize: 14 }}>{selectedTag ?? 'All tags'}</Text>
                <Text style={{ color: '#64748B', fontSize: 12 }}>{open ? '▲' : '▼'}</Text>
            </Pressable>

            {open && (
                <View style={{
                    position: 'absolute', top: 44, left: 16,
                    minWidth: 180, backgroundColor: '#FFFFFF',
                    borderRadius: 10, borderWidth: 1, borderColor: '#E2E8F0',
                    shadowColor: '#000', shadowOpacity: 0.1, shadowRadius: 8,
                    shadowOffset: { width: 0, height: 4 }, elevation: 8, zIndex: 20, overflow: 'hidden',
                }}>
                    <ScrollView keyboardShouldPersistTaps="handled" style={{ maxHeight: 240 }}>
                        <Pressable
                            onPress={() => { onSelect(null); setOpen(false); }}
                            style={{ paddingHorizontal: 14, paddingVertical: 10, backgroundColor: selectedTag === null ? '#F1F5F9' : '#FFFFFF' }}
                        >
                            <Text style={{ color: '#1E293B', fontSize: 14, fontWeight: selectedTag === null ? '600' : '400' }}>All tags</Text>
                        </Pressable>
                        {tags.map(tag => (
                            <Pressable
                                key={tag.title}
                                onPress={() => { onSelect(tag.title); setOpen(false); }}
                                style={{
                                    flexDirection: 'row', alignItems: 'center', gap: 8,
                                    paddingHorizontal: 14, paddingVertical: 10,
                                    backgroundColor: selectedTag === tag.title ? '#F1F5F9' : '#FFFFFF',
                                }}
                            >
                                <View style={{ width: 10, height: 10, borderRadius: 5, backgroundColor: tag.colorHex }} />
                                <Text style={{ color: '#1E293B', fontSize: 14, fontWeight: selectedTag === tag.title ? '600' : '400' }}>
                                    {tag.title}
                                </Text>
                            </Pressable>
                        ))}
                    </ScrollView>
                </View>
            )}
        </View>
    );
}