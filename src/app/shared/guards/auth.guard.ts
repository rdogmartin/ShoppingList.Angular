import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { inject } from '@angular/core';
import { map } from 'rxjs';

export const authGuard: CanActivateFn = () => {
  // Return true when the user is logged in; otherwise, redirect to the home page.
  const router = inject(Router);

  return inject(AuthService).isLoggedIn$.pipe(map((isLoggedIn) => isLoggedIn || router.createUrlTree(['/'])));
};
