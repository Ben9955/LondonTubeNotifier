export type User = {
  username: string;
  email: string;
  fullName?: string;
  phoneNumber?: string;
  pushNotifications: boolean;
  emailNotifications: boolean;
};
