import { Inject, Injectable, PLATFORM_ID } from '@angular/core';

import { BrowserStorageService } from './browser-storage.service';

@Injectable({
  providedIn: 'root',
})
export class BrowserLocalStorageService extends BrowserStorageService {
  public constructor(@Inject(PLATFORM_ID) platformId: object) {
    super(platformId, window.localStorage);
  }
}
