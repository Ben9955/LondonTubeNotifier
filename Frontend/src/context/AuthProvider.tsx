import { useState, useEffect } from "react";
import type { ReactNode } from "react";
import { AuthContext } from "./AuthContext";
import { getAccessToken, refreshAccessToken } from "../services/authService";
import type { User } from "../types/user";

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);

  const isAuthenticated = !!user;

  useEffect(() => {
    async function tryRefresh() {
      try {
        const data = await refreshAccessToken();
        setUser(data.user);
      } catch (err) {
        setUser(null);
      }
    }

    if (!user && getAccessToken()) {
      tryRefresh();
    }
  }, []);

  return (
    <AuthContext.Provider value={{ user, setUser, isAuthenticated }}>
      {children}
    </AuthContext.Provider>
  );
}
