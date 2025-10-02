import { useState, useEffect } from "react";
import { SubscriptionContext } from "./SubscriptionContext";
import { useAuth } from "../hooks/useAuth";
import api from "../services/apiClient";
import type { Line } from "../types/line";

export function SubscriptionProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const { isAuthenticated } = useAuth();
  const [subscribedLines, setSubscribedLines] = useState<Line[]>([]);

  const refreshSubscriptions = async () => {
    if (!isAuthenticated) {
      setSubscribedLines([]);
      return;
    }
    try {
      const res = await api.get("/subscriptions");
      setSubscribedLines(res.data);
    } catch (err) {
      console.error("Failed to fetch subscriptions", err);
    }
  };

  useEffect(() => {
    refreshSubscriptions();
  }, [isAuthenticated]);

  const toggleSubscription = async (line: Line, current: boolean) => {
    console.log(line);
    try {
      if (current) {
        await api.delete(`/subscriptions/line/${line.id}`);
        setSubscribedLines((prev) => prev.filter((l) => l.id !== line.id));
      } else {
        await api.post(`/subscriptions/line/${line.id}`);
        setSubscribedLines((prev) => [...prev, line]);
      }
    } catch (err) {
      console.error("Subscription toggle failed", err);
    }
  };

  return (
    <SubscriptionContext.Provider
      value={{ subscribedLines, toggleSubscription, refreshSubscriptions }}
    >
      {children}
    </SubscriptionContext.Provider>
  );
}
