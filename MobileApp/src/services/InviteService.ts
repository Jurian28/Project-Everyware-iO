import config from '../config';
import AuthService from './AuthService';

const baseUrl = config.apiBaseUrl;

export class InviteService {
  public static async acceptInvite(token: string): Promise<{ success: boolean; message: string }> {
    try {
      const res = await fetch(`${baseUrl}/api/invites/accept/${encodeURIComponent(token.toUpperCase())}`, {
        method: 'POST',
        headers: await AuthService.getAuthHeaders(),
      });

      const json = await res.json().catch(() => null);

      if (res.ok) {
        return { success: true, message: 'Successfully joined the event!' };
      }

      return {
        success: false,
        message: json?.error ?? 'Failed to accept the invite.',
      };
    } catch {
      return { success: false, message: 'Network error. Please try again.' };
    }
  }
}
