import { CommonModule } from '@angular/common';
import {} from '@angular/common/http';
import { Component } from '@angular/core';
import { combineLatest, map } from 'rxjs';
import { AppInfoService } from '../../services/appInfo.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-footer',
  imports: [CommonModule],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss',
})
export class FooterComponent {
  model$ = combineLatest([this.appInfoService.getAppInfo(), this.authService.getCurrentUser()]).pipe(
    map(([appInfo, user]) => ({ appInfo, user })),
  );

  constructor(
    private appInfoService: AppInfoService,
    private authService: AuthService,
  ) {}
}
