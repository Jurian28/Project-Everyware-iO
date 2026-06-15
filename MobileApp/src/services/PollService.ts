import config from '../config';
import AuthService from './AuthService';

const baseUrl = config.apiBaseUrl;

export type PollAnswerDTO = {
  idPollAnswer: number;
  text: string;
  voteCount: number;
};

export type PollDTO = {
  idPoll: number;
  title: string;
  description?: string;
  isClosed: boolean;
  idSession: number;
  hasVoted: boolean;
  votedAnswerId: number | null;
  answers: PollAnswerDTO[];
};

async function safeJson(res: Response) {
  try {
    return await res.json();
  } catch {
    return null;
  }
}

export class PollService {
  public static async getSessionPolls(
    sessionId: number,
  ): Promise<{ success: boolean; polls?: PollDTO[]; message?: string }> {
    try {
      const response = await fetch(
        `${baseUrl}/poll/session/${sessionId}`,
        {
          method: 'GET',
          headers: await AuthService.getAuthHeaders(),
        },
      );

      if (!response.ok) {
        const message = await response.text();
        return { success: false, message };
      }

      const json = await response.json();

      if (!json.success) {
        return { success: false, message: json.error ?? 'Failed to fetch polls' };
      }

      return { success: true, polls: json.data };
    } catch (error) {
      return { success: false, message: 'Internal Server Error' };
    }
  }

  public static async getPoll(
    pollId: number,
  ): Promise<{ success: boolean; poll?: PollDTO; message?: string }> {
    try {
      const response = await fetch(`${baseUrl}/poll/${pollId}`, {
        method: 'GET',
        headers: await AuthService.getAuthHeaders(),
      });

      if (!response.ok) {
        const message = await response.text();
        return { success: false, message };
      }

      const json = await response.json();

      if (!json.success) {
        return { success: false, message: json.error ?? 'Failed to fetch poll' };
      }

      return { success: true, poll: json.data };
    } catch (error) {
      return { success: false, message: 'Internal Server Error' };
    }
  }

  public static async createPoll(
    title: string,
    description: string | undefined,
    answers: string[],
    sessionId: number,
  ): Promise<{ success: boolean; poll?: PollDTO; message?: string }> {
    try {
      const response = await fetch(`${baseUrl}/poll`, {
        method: 'POST',
        headers: await AuthService.getAuthHeaders(),
        body: JSON.stringify({
          title,
          description,
          answers,
          idSession: sessionId,
        }),
      });

      const data = await safeJson(response);

      if (!response.ok) {
        return {
          success: false,
          message: data?.error ?? data?.message ?? 'Failed to create poll',
        };
      }

      if (!data?.success) {
        return { success: false, message: data?.error ?? 'Creation failed' };
      }

      return { success: true, poll: data.data };
    } catch (error) {
      return { success: false, message: 'Internal Server Error' };
    }
  }

  public static async deletePoll(
    pollId: number,
  ): Promise<{ success: boolean; message?: string }> {
    try {
      const response = await fetch(`${baseUrl}/poll/${pollId}`, {
        method: 'DELETE',
        headers: await AuthService.getAuthHeaders(),
      });

      const data = await safeJson(response);

      if (!response.ok) {
        return {
          success: false,
          message: data?.error ?? data?.message ?? 'Failed to delete poll',
        };
      }

      if (!data?.success) {
        return { success: false, message: data?.error ?? 'Delete failed' };
      }

      return { success: true };
    } catch (error) {
      return { success: false, message: 'Internal Server Error' };
    }
  }

  public static async closePoll(
    pollId: number,
  ): Promise<{ success: boolean; poll?: PollDTO; message?: string }> {
    try {
      const response = await fetch(`${baseUrl}/poll/${pollId}/close`, {
        method: 'POST',
        headers: await AuthService.getAuthHeaders(),
      });

      const data = await safeJson(response);

      if (!response.ok) {
        return {
          success: false,
          message: data?.error ?? data?.message ?? 'Failed to close poll',
        };
      }

      if (!data?.success) {
        return { success: false, message: data?.error ?? 'Close failed' };
      }

      return { success: true, poll: data.data };
    } catch (error) {
      return { success: false, message: 'Internal Server Error' };
    }
  }

  public static async vote(
    pollId: number,
    idPollAnswer: number,
  ): Promise<{ success: boolean; poll?: PollDTO; message?: string }> {
    try {
      const response = await fetch(`${baseUrl}/poll/${pollId}/vote`, {
        method: 'POST',
        headers: await AuthService.getAuthHeaders(),
        body: JSON.stringify({ idPollAnswer }),
      });

      const data = await safeJson(response);

      if (!response.ok) {
        return {
          success: false,
          message: data?.error ?? data?.message ?? 'Failed to vote',
        };
      }

      if (!data?.success) {
        return { success: false, message: data?.error ?? 'Vote failed' };
      }

      return { success: true, poll: data.data };
    } catch (error) {
      return { success: false, message: 'Internal Server Error' };
    }
  }
}
