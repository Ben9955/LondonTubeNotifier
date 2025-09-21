import axios from "axios";
import type { LoginPayload, RegisterPayload } from "../types/auth";
import type { User } from "../types/user";

// accessToken
let accessToken: string | null = null;

export function getAccessToken() {
  return accessToken;
}

function setAccessToken(token: string) {
  accessToken = token;
}

let isRefreshing = false;
let failedQueue: any[] = [];

// Function to process the queue of failed requests
const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });

  failedQueue = [];
};

const api = axios.create({
  baseURL: "https://localhost:7284/api",
  withCredentials: true,
});

// Request Interceptor: Add the access token to the Authorization header
api.interceptors.request.use((config) => {
  const token = getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response Interceptor: Handle automatic token refreshing
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response.status === 401 && !originalRequest._retry) {
      if (!getAccessToken()) {
        logout();
        return Promise.reject(error);
      }

      // Mark the original request for retry
      originalRequest._retry = true;

      // Check if another refresh request is already in progress
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then((token) => {
            originalRequest.headers.Authorization = `Bearer ${token}`;
            return axios(originalRequest);
          })
          .catch((err) => Promise.reject(err));
      }

      isRefreshing = true;

      try {
        const newAccessToken = await refreshAccessToken();

        if (newAccessToken) {
          //set accessToken in memory
          setAccessToken(newAccessToken);

          // Retry all queued requests with the new token
          processQueue(null, newAccessToken);

          // Retry the original failed request
          originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
          return axios(originalRequest);
        }
      } catch (err) {
        // If the refresh token request fails, it means the user needs to log in again
        processQueue(err);
        logout();
        return Promise.reject(err);
      } finally {
        isRefreshing = false;
      }
    }
    // For all other errors, just reject the promise
    return Promise.reject(error);
  }
);

export default api;

export async function register(requestData: RegisterPayload): Promise<User> {
  const res = await api.post("account/register", requestData, {
    headers: {
      "Content-Type": "application/json",
    },
    withCredentials: true,
  });

  const data = res.data;

  console.log(data);

  setAccessToken(data.accessToken);
  const user: User = {
    username: data.userName,
    email: data.email,
    fullName: data.fullName,
    subscriptions: data.subscriptions ?? [],
  };

  return user;
}

export async function login(requestData: LoginPayload) {
  const res = await api.post("account/login", requestData, {
    headers: {
      "Content-Type": "application/json",
    },
    withCredentials: true,
  });

  const data = res.data;

  console.log(data);

  setAccessToken(data.accessToken);
  const user: User = {
    username: data.userName,
    email: data.email,
    fullName: data.fullName,
    subscriptions: data.subscriptions ?? [],
  };

  return user;
}

export async function refreshAccessToken() {
  const res = await api.post(
    "account/generate-new-jwt-token",
    {},
    {
      withCredentials: true,
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    }
  );
  setAccessToken(res.data.accessToken);
  return res.data.accessToken;
}

export async function logout() {
  accessToken = null;
  await api.post("account/logout", { withCredentials: true });
}
