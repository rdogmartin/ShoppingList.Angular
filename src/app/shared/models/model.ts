export interface CurrentUser {
  userId: string;
  userRoles: string[];
  identityProvider: string;
  userDetails: string;
  claims: string[];
}

export interface AuthResult {
  clientPrincipal?: CurrentUser;
}
