import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { first, Observable, switchMap } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor2 implements HttpInterceptor {
  public constructor(private authService: AuthService) {}

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  public intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return this.authService.getCurrentUser().pipe(
      first(), // We want this to only fire once so that the finalize runs in spinner-interceptor.ts
      switchMap((user) => {
        const requestWithClientId = request.clone({
          setHeaders: {
            ['x-tis-auth-user']: user.userName,
          },
        });
        return next.handle(requestWithClientId);
      }),
    );
  }
}
