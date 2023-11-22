import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { map } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { BrowserService } from '../services/browser.service';

export const authGuard: CanActivateFn = () => {
  // Return true when the user is logged in; otherwise, redirect to the login page.
  const browserService = inject(BrowserService);

  return inject(AuthService).isLoggedIn$.pipe(
    map((isLoggedIn) => {
      if (isLoggedIn) {
        return true;
      } else {
        browserService.redirectToPage('/.auth/login/aad');
        return false;
      }
    }),
  );
};
