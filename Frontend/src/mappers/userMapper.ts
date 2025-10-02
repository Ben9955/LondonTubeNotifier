import type { User } from "../types/user";

export function mapToUser(data: any): User {
  return {
    username: data.userName,
    email: data.email,
    fullName: data.fullName,
    phoneNumber: data.phoneNumber,
    pushNotifications: data.pushNotifications,
    emailNotifications: data.emailNotifications,
  };
}
