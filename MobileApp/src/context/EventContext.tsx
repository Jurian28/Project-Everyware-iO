import React, { createContext, useContext, useState } from 'react';

type EventContextValue = {
  currentEventName: string | null;
  setCurrentEventName: (name: string | null) => void;
};

const EventContext = createContext<EventContextValue>({
  currentEventName: null,
  setCurrentEventName: () => {},
});

export function EventProvider({ children }: { children: React.ReactNode }) {
  const [currentEventName, setCurrentEventName] = useState<string | null>(null);
  return (
    <EventContext.Provider value={{ currentEventName, setCurrentEventName }}>
      {children}
    </EventContext.Provider>
  );
}

export function useEventContext() {
  return useContext(EventContext);
}
