import * as Keychain from 'react-native-keychain';
import config from '../config';

export default class AuthService {
  private static readonly _authServiceKey = 'auth';

  public static async login(email: string, password: string): Promise<boolean> {
    try {
      const response = await fetch(`${config.apiBaseUrl}/api/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password }),
      });

      if (!response.ok) {
        console.error('Login request failed', response.status);
        return false;
      }

      const json = await response.json();

      if (!json.accessToken || !json.refreshToken) {
        console.error('Invalid login response', json);
        return false;
      }

      const success = await Keychain.setGenericPassword(
        email,
        json.accessToken,
        {
          service: AuthService._authServiceKey,
        },
      );

      if (!success) {
        console.error('Failed to store credentials');
        return false;
      }

      return true;
    } catch (error) {
      console.error('Login failed', error);
      return false;
    }
  }

  public static async register(
    email: string,
    password: string,
  ): Promise<boolean> {
    try {
      const response = await fetch(`${config.apiBaseUrl}/api/auth/register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password }),
      });

      if (!response.ok) {
        console.error('Registration request failed', response.status);
        return false;
      }

      return true;
    } catch (error) {
      console.error('Registration failed', error);
      return false;
    }
  }

  public static async logout(): Promise<void> {
    try {
      const token = await Keychain.getGenericPassword({
        service: AuthService._authServiceKey,
      });

      if (token) {
        await fetch(`${config.apiBaseUrl}/api/auth/logout`, {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token.password}`,
          },
        });

        await Keychain.resetGenericPassword({
          service: AuthService._authServiceKey,
        });
      }
    } catch (error) {
      console.error('Logout failed', error);
    }
  }

  public static async isAuthenticated(): Promise<boolean> {
    try {
      const credentials = await Keychain.getGenericPassword({
        service: AuthService._authServiceKey,
      });
      return !!credentials;
    } catch (error) {
      console.error('Failed to check authentication', error);
      return false;
    }
  }
}
