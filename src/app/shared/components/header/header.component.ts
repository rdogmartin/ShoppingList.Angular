import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from '../../services/auth.service';
import { BrowserLocalStorageService } from '../../services/browser-local-storage.service';
import { BrowserService } from '../../services/browser.service';

@Component({
  selector: 'app-header',
  imports: [CommonModule, MatIconModule, MatMenuModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent {
  public isLoggedIn = toSignal(this.authService.isLoggedIn$, {
    initialValue: false,
    rejectErrors: true,
  });

  constructor(
    private authService: AuthService,
    private browserLocalStorageService: BrowserLocalStorageService,
    private browserService: BrowserService,
  ) {}

  public logout() {
    this.browserLocalStorageService.clear();
    this.browserService.redirectToPage('/');
  }
}
