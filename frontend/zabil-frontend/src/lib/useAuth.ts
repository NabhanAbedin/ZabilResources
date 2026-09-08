import { useSyncExternalStore } from "react";
import type { AuthSnapshot } from "../types/interfaces";
import { getSnapshot, subscribe } from "./authSession";

export const useAuth = (): AuthSnapshot =>
  useSyncExternalStore(subscribe, getSnapshot, getSnapshot);
