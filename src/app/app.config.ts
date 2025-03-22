import { ApplicationConfig, ErrorHandler } from '@angular/core';
import { provideRouter } from '@angular/router';

import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
import { routes } from './app.routes';
import { GlobalErrorHandler } from './shared/error-handling/global-error-handler';
import { authInterceptorInterceptor } from './shared/interceptors/auth-interceptor.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptorInterceptor])),
    provideAnimations(),
    provideToastr(),
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
  ],
};
