import { Injectable } from '@angular/core';
import { AuthResult } from '../models/model';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private http: HttpClient) {}

  public getCurrentUser() {
    return this.http.get<AuthResult>('/.auth/me').pipe(
      map(
        (authResult) =>
          authResult.clientPrincipal || {
            userId: '',
            userRoles: [],
            identityProvider: '',
            userDetails: '',
            claims: [],
          },
      ),
    );
  }

  public readonly isLoggedIn$ = this.getCurrentUser().pipe(map((u) => u.userId !== ''));
}
