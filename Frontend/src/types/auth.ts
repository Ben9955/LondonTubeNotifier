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
