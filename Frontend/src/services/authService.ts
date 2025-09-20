import axios from "axios";
import type { LoginPayload, RegisterPayload } from "../types/auth";

const API_URL = " https://localhost:7284/api/account";

export async function register(data: RegisterPayload) {
  const res = await axios.post(`${API_URL}/register`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });
  console.log(res.data);
  return res.data;
}

export async function login(data: LoginPayload) {
  const res = await axios.post(`${API_URL}/login`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });
  console.log(res.data);
  return res.data;
}
