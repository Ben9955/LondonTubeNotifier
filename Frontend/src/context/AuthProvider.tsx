import { useState, useEffect } from "react";
import type { ReactNode } from "react";
import { AuthContext } from "./AuthContext";
import {
  logout as apiLogout,
  refreshAccessToken,
} from "../services/authService";
import { setLogoutCallback } from "../services/apiClient";
import type { User } from "../types/user";

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const isAuthenticated = !!user;

  useEffect(() => {
    setLogoutCallback(handleLogout);
    async function tryRefresh() {
      try {
        const data = await refreshAccessToken();
        console.log(data);
        setUser(data);
      } catch (err) {
        await handleLogout();
      } finally {
        setLoading(false);
      }
    }

    if (!user) {
      tryRefresh();
    }
  }, []);

  const handleLogout = async () => {
    await apiLogout();
    setUser(null);
  };

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <AuthContext.Provider value={{ user, setUser, isAuthenticated }}>
      {children}
    </AuthContext.Provider>
  );
}
