import * as Keychain from 'react-native-keychain';
import config from '../config';

type AuthResult = {
  success: boolean;
  message?: string;
};

export default class AuthService {
  private static readonly _authServiceKey = 'auth';
  private static readonly _refreshTokenServiceKey = 'auth_refresh';

  public static async login(
    email: string,
    password: string,
  ): Promise<AuthResult> {
    try {
      const response = await fetch(`${config.apiBaseUrl}/api/auth/login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password }),
      });

      if (!response.ok) {
        const message = await response.text();
        console.error('Login request failed', response.status, message);
        return { success: false, message };
      }

      const json = await response.json();

      if (!json.accessToken || !json.refreshToken) {
        console.error('Invalid login response', json);
        return {
          success: false,
          message: 'Invalid login response from the server.',
        };
      }

      const accessTokenStored = await Keychain.setGenericPassword(
        email,
        json.accessToken,
        {
          service: AuthService._authServiceKey,
        },
      );

      const refreshTokenStored = await Keychain.setGenericPassword(
        email,
        json.refreshToken,
        {
          service: AuthService._refreshTokenServiceKey,
        },
      );

      if (!accessTokenStored || !refreshTokenStored) {
        console.error('Failed to store credentials');
        return {
          success: false,
          message: 'Failed to store credentials securely.',
        };
      }

      return { success: true };
    } catch (error) {
      console.error('Login failed', error);
      return {
        success: false,
        message: 'Login failed. Please check your connection and try again.',
      };
    }
  }

  public static async register(
    email: string,
    password: string,
  ): Promise<AuthResult> {
    try {
      const response = await fetch(`${config.apiBaseUrl}/api/auth/register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password }),
      });

      if (!response.ok) {
        const message = await response.text();
        console.error('Registration request failed', response.status, message);
        return { success: false, message };
      }

      const json = await response.json();

      if (!json.accessToken || !json.refreshToken) {
        console.error('Invalid registration response', json);
        return {
          success: false,
          message: 'Invalid registration response from the server.',
        };
      }

      const accessTokenStored = await Keychain.setGenericPassword(
        email,
        json.accessToken,
        {
          service: AuthService._authServiceKey,
        },
      );

      const refreshTokenStored = await Keychain.setGenericPassword(
        email,
        json.refreshToken,
        {
          service: AuthService._refreshTokenServiceKey,
        },
      );

      if (!accessTokenStored || !refreshTokenStored) {
        console.error('Failed to store credentials');
        return {
          success: false,
          message: 'Failed to store credentials securely.',
        };
      }

      return { success: true };
    } catch (error) {
      console.error('Registration failed', error);
      return {
        success: false,
        message:
          'Registration failed. Please check your connection and try again.',
      };
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
      }

      await Keychain.resetGenericPassword({
        service: AuthService._authServiceKey,
      });
      await Keychain.resetGenericPassword({
        service: AuthService._refreshTokenServiceKey,
      });
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
