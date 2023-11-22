import { isPlatformBrowser } from '@angular/common';
import { Inject, Injectable, PLATFORM_ID } from '@angular/core';
import { InjectionTokens } from '../utils/injection-tokens';

@Injectable({
  providedIn: 'root',
})
export class BrowserService {
  private readonly _isRenderingInBrowser: boolean;

  public constructor(
    @Inject(PLATFORM_ID) private platformId: object,
    @Inject(InjectionTokens.LOCATION) private location: Location,
  ) {
    this._isRenderingInBrowser = isPlatformBrowser(platformId);
  }

  /**
   * Redirect to an external URL. This method is only available when running in the browser.
   * @param href The URL to redirect to. Examples: https://www.google.com, /.auth/login/aad.
   */
  public redirectToPage(href: string) {
    if (!this._isRenderingInBrowser) {
      return;
    }

    this.location.href = href;
  }
}
