import api, { setAccessToken } from "./apiClient";
import type { LoginPayload, RegisterPayload } from "../types/auth";
import type { User } from "../types/user";
import { mapToUser } from "../mappers/userMapper";

export async function register(requestData: RegisterPayload): Promise<User> {
  const res = await api.post("account/register", requestData, {
    headers: { "Content-Type": "application/json" },
  });

  setAccessToken(res.data.accessToken);
  return mapToUser(res.data);
}

export async function login(requestData: LoginPayload): Promise<User> {
  const res = await api.post("account/login", requestData, {
    headers: { "Content-Type": "application/json" },
  });

  setAccessToken(res.data.accessToken);
  return mapToUser(res.data);
}

export async function refreshAccessToken(): Promise<User> {
  const res = await api.post(
    "account/generate-new-jwt-token",
    {},
    { withCredentials: true }
  );
  setAccessToken(res.data.accessToken);
  return mapToUser(res.data);
}

export async function logout() {
  setAccessToken(null);
  try {
    await api.post("account/logout", {}, { withCredentials: true });
  } catch (err) {
    console.log(err);
  }
}

export async function updateProfile(updatedUser: Partial<User>): Promise<User> {
  const res = await api.put("account/update-profile", updatedUser, {
    headers: { "Content-Type": "application/json" },
  });

  return mapToUser(res.data);
}
