import type { DecodedAuthClaims } from "../types/interfaces";

export const TOKEN_STORAGE_KEY = "zabil_jwt";

export const EXPIRY_SKEW_MS = 30_000;

export const saveToken = (token: string): void => {
  localStorage.setItem(TOKEN_STORAGE_KEY, token);
};

export const getToken = (): string | null => {
  return localStorage.getItem(TOKEN_STORAGE_KEY);
};

export const clearToken = (): void => {
  localStorage.removeItem(TOKEN_STORAGE_KEY);
};

function decodeBase64Url(segment: string): string {
  const base64 = segment.replace(/-/g, "+").replace(/_/g, "/");
  const padded = base64.padEnd(
    base64.length + ((4 - (base64.length % 4)) % 4),
    "=",
  );
  return atob(padded);
}

// Decodes the payload only — never verifies the signature, so this is a UX
// gate, not a security boundary. Real enforcement belongs server-side.
export const decodeToken = (): DecodedAuthClaims | null => {
  const token = getToken();
  if (!token) return null;

  const parts = token.split(".");
  if (parts.length !== 3) return null;

  try {
    return JSON.parse(decodeBase64Url(parts[1])) as DecodedAuthClaims;
  } catch {
    return null;
  }
};

export const getExpiresAt = (): number | null => {
  const exp = decodeToken()?.exp;
  return typeof exp === "number" ? exp * 1000 : null;
};

export const isTokenExpired = (): boolean => {
  const expiresAt = getExpiresAt();
  if (expiresAt === null) return true;
  return Date.now() >= expiresAt - EXPIRY_SKEW_MS;
};

export const hasValidToken = (): boolean => !isTokenExpired();
