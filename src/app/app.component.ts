import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { HttpClient, HttpClientModule, HttpHeaders } from '@angular/common/http';
import { catchError, of, tap } from 'rxjs';
import { AuthResult, CurrentUser } from './shared/models/model';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, HttpClientModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  currentUser: CurrentUser = { userId: '', userRoles: [], identityProvider: '', userDetails: '', claims: [] };
  message = '';
  title = 'ShoppingList.Angular';

  constructor(private http: HttpClient) {
    this.http.get<AuthResult>('/.auth/me').pipe(
      tap((authResult) => {
        console.log(authResult);
        this.currentUser = authResult.clientPrincipal;
        this.getAppInfo();
      }),
      catchError((err) => {
        console.error(err.error.error.message);
        return of('Error');
      }),
    ).subscribe();
  }

  private getAppInfo() {
    const httpOptions = {
      headers: new HttpHeaders({
        'Accept': 'text/html, application/xhtml+xml, */*',
        'Content-Type': 'application/x-www-form-urlencoded'
      }),
      responseType: 'text' as 'json'
    };

    this.http.get<string>(`/api/AppInfo?name=${this.currentUser.userDetails}`, httpOptions).pipe(
      tap((resp) => {
        console.log(resp);
        this.message = resp;
      }),
      catchError((err) => {
        console.error(err.error.error.message);
        return of('Error');
      }),
    ).subscribe();

  }
}
