export function parseSessionQRCode(qrCode: string): { sessionId: number; userId: string } | null {
    const prefix = 'io-event-connecter://session/';
    if (!qrCode.startsWith(prefix)) {
        return null;
    }

    const parts = qrCode.slice(prefix.length).split('/user/');
    if (parts.length !== 2) {
        return null;
    }

    return { sessionId: parseInt(parts[0], 10), userId: parts[1] };
}