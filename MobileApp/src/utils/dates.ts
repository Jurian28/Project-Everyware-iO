
const formatDate = (date: Date) => {
    return date.toLocaleTimeString([], {
            hour: '2-digit',
            minute: '2-digit',
        });
}

export function formatTimeRange(start: string, end: string) {
    return `${formatDate(new Date(start))} - ${formatDate(new Date(end))}`;
};

export function formatTime(time: string) {
    return `${formatDate(new Date(time))}`;
};

