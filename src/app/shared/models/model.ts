import { StorageItemKey } from './enum';

export interface CurrentUser {
  userName: string;
  isAuthenticated: boolean;
}

// export interface CurrentUser {
//   userId: string;
//   userRoles: string[];
//   identityProvider: string;
//   userDetails: string;
//   claims: string[];
// }

export interface AuthRequest {
  userName: string;
}

export interface AuthResult {
  userName: string;
  isAuthenticated: boolean;
}

export interface StorageItem {
  key: StorageItemKey;
  value: string;
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
  imageUrl: string;
  isComplete: boolean;
}

export interface ListItemViewModel extends ListItem {
  isBeingEdited: boolean;
  newDescription: string;
}

export interface ListItemAdd {
  description: string;
}

export interface ListItemUpdate {
  description: string;
  isComplete: boolean;
  newDescription: string;
}
