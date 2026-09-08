import { endSession } from "../lib/authSession";
import { getToken, isTokenExpired } from "../lib/authToken";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL;

export class ApiError extends Error {
  readonly status: number;

  constructor(message: string, status: number) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

const SESSION_ENDED_MESSAGE = "Your session has ended. Please sign in again.";

export async function apiFetch(
  path: string,
  init: RequestInit = {},
): Promise<Response> {
  const token = getToken();

  if (token !== null && isTokenExpired()) {
    endSession("expired");
    throw new ApiError(SESSION_ENDED_MESSAGE, 401);
  }

  const headers = new Headers(init.headers);
  if (token !== null) {
    headers.set("Authorization", `Bearer ${token}`);
  }

  const response = await fetch(`${API_BASE_URL}${path}`, { ...init, headers });

  if (response.status === 401) {
    endSession("unauthorized");
    throw new ApiError(SESSION_ENDED_MESSAGE, response.status);
  }

  return response;
}
