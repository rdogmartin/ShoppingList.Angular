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

export interface AppInfo {
  versionNumber: string;
}

export interface UserListItems {
  id: string;
  listItems: ListItem[];
}

export interface ListItem {
  description: string;
  isComplete: boolean;
}

export interface ListItemUpdate extends ListItem {
  newDescription: string;
}

export interface ListItemViewModel extends ListItem {
  isBeingEdited: boolean;
  newDescription: string;
}
