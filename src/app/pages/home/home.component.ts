import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { map } from 'rxjs';
import { AuthResult } from '../../shared/models/model';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss',
})
export class HomeComponent {
  currentUser = toSignal(this.getUser(), {
    initialValue: { userId: '', userRoles: [], identityProvider: '', userDetails: '', claims: [] },
    requireSync: false,
  });

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
}
