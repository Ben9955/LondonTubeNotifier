import type { User } from "./sss";

export type RegisterPayload = {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  fullName?: string;
  phoneNumber?: string;
};

export type LoginPayload = {
  emailOrUsername: string;
  password: string;
};

export type AuthContextType = {
  user: User | null;
  setUser: (user: User | null) => void;
  isAuthenticated: boolean;
};
