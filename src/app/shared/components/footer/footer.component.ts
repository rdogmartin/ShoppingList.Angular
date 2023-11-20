import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Component } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { AppInfoService } from '../../services/appInfo.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss',
})
export class FooterComponent {
  appInfo$ = toSignal(this.appInfoService.getAppInfo());
  isLoggedIn$ = toSignal(this.authService.isLoggedIn$);

  constructor(
    private appInfoService: AppInfoService,
    private authService: AuthService,
  ) {}
}
