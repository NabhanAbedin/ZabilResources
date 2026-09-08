import type { AuthSnapshot, SessionEndedReason } from "../types/interfaces";
import {
  EXPIRY_SKEW_MS,
  TOKEN_STORAGE_KEY,
  clearToken,
  decodeToken,
  getExpiresAt,
  hasValidToken,
  saveToken,
} from "./authToken";


const CHANNEL_NAME = "zabil_auth";

// setTimeout clamps anything past a signed 32-bit millisecond delay.
const MAX_TIMEOUT_MS = 2_147_483_647;

type AuthMessage =
  | { type: "signed-in" }
  | { type: "ended"; reason: SessionEndedReason };

type Listener = () => void;

const listeners = new Set<Listener>();
const supportsChannel = typeof BroadcastChannel !== "undefined";

let snapshot: AuthSnapshot = readSnapshot(null);
let expiryTimer: ReturnType<typeof setTimeout> | null = null;
let channel: BroadcastChannel | null = null;
let started = false;

function readSnapshot(endedReason: SessionEndedReason | null): AuthSnapshot {
  const claims = hasValidToken() ? decodeToken() : null;

  return {
    isLoggedIn: claims !== null,
    isAdmin: claims?.Role === "Admin",
    email: claims?.Email ?? null,
    endedReason,
  };
}

function emit(next: AuthSnapshot): void {
  const prev = snapshot;
  const unchanged =
    prev.isLoggedIn === next.isLoggedIn &&
    prev.isAdmin === next.isAdmin &&
    prev.email === next.email &&
    prev.endedReason === next.endedReason;

  if (unchanged) return;

  snapshot = next;
  listeners.forEach((listener) => listener());
}

function clearExpiryTimer(): void {
  if (expiryTimer === null) return;
  clearTimeout(expiryTimer);
  expiryTimer = null;
}

function scheduleExpiryCheck(): void {
  clearExpiryTimer();

  const expiresAt = getExpiresAt();
  if (expiresAt === null) return;

  const delay = Math.min(
    Math.max(expiresAt - EXPIRY_SKEW_MS - Date.now(), 0),
    MAX_TIMEOUT_MS,
  );
  expiryTimer = setTimeout(checkSession, delay);
}

// Clears local state without telling the other tabs — used when we are already
// reacting to their message, so the broadcast does not echo back and forth.
function applyEnded(reason: SessionEndedReason): void {
  clearExpiryTimer();
  clearToken();
  emit(readSnapshot(reason));
}

/**
 * The one predicate, called from every trigger: the expiry timer, tab refocus,
 * app boot, and before each outgoing request.
 */
export function checkSession(): void {
  if (decodeToken() === null) {
    // Nothing usable in storage; nothing to expire.
    clearExpiryTimer();
    emit(readSnapshot(snapshot.endedReason));
    return;
  }

  if (hasValidToken()) {
    // Re-arm: a timer set before the tab was backgrounded may have been
    // throttled, and timers do not advance while the machine is asleep.
    scheduleExpiryCheck();
    return;
  }

  endSession("expired");
}

export function endSession(reason: SessionEndedReason): void {
  applyEnded(reason);
  channel?.postMessage({ type: "ended", reason } satisfies AuthMessage);
}

export function signIn(token: string): void {
  saveToken(token);
  emit(readSnapshot(null));
  scheduleExpiryCheck();
  channel?.postMessage({ type: "signed-in" } satisfies AuthMessage);
}

export const getSnapshot = (): AuthSnapshot => snapshot;

export function subscribe(listener: Listener): () => void {
  listeners.add(listener);
  return () => {
    listeners.delete(listener);
  };
}

function handleWake(): void {
  if (document.visibilityState !== "visible") return;
  checkSession();
}

function handleChannelMessage(event: MessageEvent<AuthMessage>): void {
  if (event.data.type === "ended") {
    applyEnded(event.data.reason);
    return;
  }

  emit(readSnapshot(null));
  scheduleExpiryCheck();
}

// Fallback for browsers without BroadcastChannel. Only registered in that case:
// the storage event cannot tell us *why* a session ended, so when the channel
// is available we let its more specific reason be the only writer.
function handleStorage(event: StorageEvent): void {
  // A null key means the whole store was cleared.
  if (event.key !== null && event.key !== TOKEN_STORAGE_KEY) return;

  if (decodeToken() === null) {
    applyEnded("signed-out");
    return;
  }

  emit(readSnapshot(null));
  scheduleExpiryCheck();
}

/**
 * Wires up the triggers. Call once at startup; returns a teardown function.
 */
export function startAuthSession(): () => void {
  if (started) return () => {};
  started = true;

  if (supportsChannel) {
    channel = new BroadcastChannel(CHANNEL_NAME);
    channel.addEventListener("message", handleChannelMessage);
  } else {
    window.addEventListener("storage", handleStorage);
  }

  document.addEventListener("visibilitychange", handleWake);
  window.addEventListener("focus", handleWake);

  // Boot check: a restored tab is already visible and never fires a focus
  // event, so this is the only thing that catches "reopened the next day".
  checkSession();

  return () => {
    document.removeEventListener("visibilitychange", handleWake);
    window.removeEventListener("focus", handleWake);

    if (channel) {
      channel.removeEventListener("message", handleChannelMessage);
      channel.close();
      channel = null;
    } else {
      window.removeEventListener("storage", handleStorage);
    }

    clearExpiryTimer();
    started = false;
  };
}
