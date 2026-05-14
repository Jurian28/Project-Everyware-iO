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

async function safeJson(res: Response) {
	try {
		return await res.json();
	} catch {
		return null;
	}
}

export default {
	async getSession(sessionId: number) {
		const res = await fetch(
			`${baseUrl}/sessions/${sessionId}`,
			{
				headers: await authHeaders(),
			}
		);

		if (!res.ok) {
			return {
				success: false,
				message: 'Session not found',
			};
		}

		const json = await res.json();
		return json.data;
	},

	async enroll(sessionId: number) {
		const res = await fetch(
			`${baseUrl}/sessions/${sessionId}/enroll`,
			{
				method: 'POST',
				headers: await authHeaders(),
			}
		);

		const data = await safeJson(res);

		if (res.status === 409) {
			return {
				success: false,
				type: 'conflict',
				conflictSession: data?.conflictSession,
				message: data?.message,
			};
		}

		if (res.status === 400) {
			return {
				success: false,
				type: 'already-enrolled',
				message:
					data?.message ??
					'Already enrolled',
			};
		}

		if (!res.ok) {
			return {
				success: false,
				message:
					data?.message ??
					'Enroll failed',
			};
		}

		return { success: true };
	},

	async withdraw(sessionId: number) {
		const res = await fetch(
			`${baseUrl}/sessions/${sessionId}/enroll`,
			{
				method: 'DELETE',
				headers: await authHeaders(),
			}
		);

		const data = await safeJson(res);

		if (!res.ok) {
			return {
				success: false,
				message:
					data?.message ??
					'Withdraw failed',
			};
		}

		return { success: true };
	},
};
