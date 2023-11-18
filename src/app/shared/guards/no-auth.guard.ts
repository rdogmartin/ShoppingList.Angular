import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { map } from 'rxjs';

export const noAuthGuard: CanActivateFn = () => {
  // Return true when user is not logged in; otherwise, redirect to the list page.
  const router = inject(Router);

  return inject(AuthService).isLoggedIn$.pipe(map((isLoggedIn) => !isLoggedIn || router.createUrlTree(['/list'])));
};
