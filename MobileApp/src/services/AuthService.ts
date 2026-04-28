import * as Keychain from 'react-native-keychain';
import config from '../config';

type AuthResult = {
  success: boolean;
  message?: string;
};

type AuthResponseBody = {
  accessToken?: string;
  refreshToken?: string;
};

type AuthAction = 'login' | 'register';

const authActionConfig: Record<
  AuthAction,
  {
    requestFailedLog: string;
    invalidResponseLog: string;
    invalidResponseMessage: string;
    failureLog: string;
    failureMessage: string;
  }
> = {
  login: {
    requestFailedLog: 'Login request failed',
    invalidResponseLog: 'Invalid login response',
    invalidResponseMessage: 'Invalid login response from the server.',
    failureLog: 'Login failed',
    failureMessage: 'Login failed. Please check your connection and try again.',
  },
  register: {
    requestFailedLog: 'Registration request failed',
    invalidResponseLog: 'Invalid registration response',
    invalidResponseMessage: 'Invalid registration response from the server.',
    failureLog: 'Registration failed',
    failureMessage:
      'Registration failed. Please check your connection and try again.',
  },
};

export default class AuthService {
  private static readonly _accessTokenServiceKey = 'auth_access_token';
  private static readonly _refreshTokenServiceKey = 'auth_refresh_token';

  private static async saveTokens(
    email: string,
    accessToken: string,
    refreshToken: string,
  ): Promise<boolean> {
    const accessTokenStored = await Keychain.setGenericPassword(
      email,
      accessToken,
      {
        service: AuthService._accessTokenServiceKey,
      },
    );

    const refreshTokenStored = await Keychain.setGenericPassword(
      email,
      refreshToken,
      {
        service: AuthService._refreshTokenServiceKey,
      },
    );

    return !!accessTokenStored && !!refreshTokenStored;
  }

  private static async submitAuth(
    action: AuthAction,
    email: string,
    password: string,
  ): Promise<AuthResult> {
    const configForAction = authActionConfig[action];

    try {
      const response = await fetch(`${config.apiBaseUrl}/api/auth/${action}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, password }),
      });

      if (!response.ok) {
        const message = await response.text();
        console.error(
          configForAction.requestFailedLog,
          response.status,
          message,
        );
        return { success: false, message };
      }

      const json: AuthResponseBody = await response.json();

      if (!json.accessToken || !json.refreshToken) {
        console.error(configForAction.invalidResponseLog, json);
        return {
          success: false,
          message: configForAction.invalidResponseMessage,
        };
      }

      const stored = await AuthService.saveTokens(
        email,
        json.accessToken,
        json.refreshToken,
      );

      if (!stored) {
        console.error('Failed to store credentials');
        return {
          success: false,
          message: 'Failed to store credentials securely.',
        };
      }

      return { success: true };
    } catch (error) {
      console.error(configForAction.failureLog, error);
      return {
        success: false,
        message: configForAction.failureMessage,
      };
    }
  }

  public static async login(
    email: string,
    password: string,
  ): Promise<AuthResult> {
    return AuthService.submitAuth('login', email, password);
  }

  public static async register(
    email: string,
    password: string,
  ): Promise<AuthResult> {
    return AuthService.submitAuth('register', email, password);
  }

  public static async logout(): Promise<void> {
    try {
      const token = await Keychain.getGenericPassword({
        service: AuthService._accessTokenServiceKey,
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
        service: AuthService._accessTokenServiceKey,
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
        service: AuthService._accessTokenServiceKey,
      });
      return !!credentials;
    } catch (error) {
      console.error('Failed to check authentication', error);
      return false;
    }
  }
}
