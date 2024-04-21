export class AuthenticatedUser {
  twoFactorEnabled?: boolean;
  token?: string;
  refreshToken?: string;
  expiration?: string;
  longTermToken?: string;
  name?: string;
}