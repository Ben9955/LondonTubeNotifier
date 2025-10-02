import { createContext } from "react";
import type { Line } from "../types/line";

export type SubscriptionContextType = {
  subscribedLines: Line[];
  toggleSubscription: (line: Line, current: boolean) => Promise<void>;
  refreshSubscriptions: () => Promise<void>;
};

export const SubscriptionContext =
  createContext<SubscriptionContextType | null>(null);
