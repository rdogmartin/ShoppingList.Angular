import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { switchMap, take } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptorInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  return authService.getCurrentUser().pipe(
    take(1), // Needed to force the observable to complete
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
