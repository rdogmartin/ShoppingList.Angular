import { InjectionToken } from '@angular/core';

export class InjectionTokens {
  public static readonly LOCATION = new InjectionToken('Location', {
    providedIn: 'root',
    factory: () => location,
  });
}
