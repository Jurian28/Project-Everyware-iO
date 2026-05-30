import * as Keychain from 'react-native-keychain';
import config from '../config';

const baseUrl = config.apiBaseUrl;



async function authHeaders() {
	const token = await Keychain.getGenericPassword({
		service: 'auth_access_token',
	});

	return {
		'Content-Type': 'application/json',
		...(token
			? {
				Authorization: `Bearer ${token.password}`,
			}
				: {}),
	};
}

export class SessionReviewService {
	public static async getReviewsForSessionId(sessionId: number) {
		try {
			const res = await fetch(`${baseUrl}/sessions/${sessionId}/reviews`, {
				method: 'GET',
				headers: await authHeaders(),
			});

			if (!res.ok) {
				const message = await res.text();
				console.error(
					`Failed to fetch reviews for session ${sessionId}:`,
					res.status,
					message,
				);
				return { success: false, message: 'Failed to fetch reviews' };
			}

			const json = await res.json();

			if (!json.success) {
				console.error(
					`API responded with success=false for session ${sessionId}:`,
						json,
				);
				return {
					success: false,
					message: 'fetch returned unsuccessful',
				};
			}

			return { success: true, reviews: json.data };

		} catch (error)	{
			return { success: false, message: 'Internal Server Error' };
		}
	}
}
