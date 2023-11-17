import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { map } from 'rxjs';
import { AppInfo, AuthResult } from './shared/models/model';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, HttpClientModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  currentUser = toSignal(this.getUser(), {
    initialValue: { userId: '', userRoles: [], identityProvider: '', userDetails: '', claims: [] },
    requireSync: false,
  });
  appInfo = toSignal(this.getAppInfo());
  message = '';
  title = 'ShoppingList.Angular';

  constructor(private http: HttpClient) {}

  private getUser() {
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

  private getAppInfo() {
    return this.http.get<AppInfo>('/api/AppInfo');
  }
}
