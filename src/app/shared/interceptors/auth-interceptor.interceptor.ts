import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { switchMap } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  return authService.getCurrentUser().pipe(
    switchMap((user) => {
      const requestWithClientId = req.clone({
        setHeaders: {
          ['x-tis-auth-token']: user.token,
        },
      });
      return next(requestWithClientId);
    }),
  );
};
