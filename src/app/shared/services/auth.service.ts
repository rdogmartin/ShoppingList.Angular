import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, tap } from 'rxjs/operators';
import { StorageItemKey } from '../models/enum';
import { AuthResult, CurrentUser } from '../models/model';
import { BrowserLocalStorageService } from './browser-local-storage.service';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(
    private http: HttpClient,
    private browserLocalStorageService: BrowserLocalStorageService,
  ) {}

  public authenticate(userName: string) {
    const authRequest = { userName: userName };
    return this.http.post<AuthResult>('/api/authenticate', authRequest).pipe(
      tap((authResult) => {
        this.setCurrentUser({ userName: authResult.userName, isAuthenticated: authResult.isAuthenticated });
      }),
    );
  }

  public getCurrentUser() {
    return this.browserLocalStorageService
      .select<CurrentUser>(StorageItemKey.CurrentUser)
      .pipe(map((value) => (value === null ? { userName: '', isAuthenticated: false } : value)));
  }

  public setCurrentUser(currentUser: CurrentUser) {
    this.browserLocalStorageService.set(StorageItemKey.CurrentUser, currentUser);
  }

  public readonly isLoggedIn$ = this.getCurrentUser().pipe(map((u) => u.isAuthenticated));
}
