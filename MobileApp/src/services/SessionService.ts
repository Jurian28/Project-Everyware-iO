import config from '../config';

export default {
	async getSession(eventId: number, sessionId: number) {
		const res = await fetch(
			`${config.apiBaseUrl}/${eventId}/sessions/${sessionId}/edit`
		);

		if (!res.ok) {
			throw new Error('Failed to load session');
		}

		const json = await res.json();

		return json.data.session;
	}
};


