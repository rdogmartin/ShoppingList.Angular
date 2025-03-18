import { A11yModule } from '@angular/cdk/a11y';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { combineLatest, map, Subscription, tap } from 'rxjs';
import { AppInfoService } from '../../services/appInfo.service';
import { AuthService } from '../../services/auth.service';
import { BrowserLocalStorageService } from '../../services/browser-local-storage.service';
import { BrowserService } from '../../services/browser.service';

@Component({
  selector: 'app-footer',
  imports: [A11yModule, CommonModule, MatButtonModule, MatInputModule, ReactiveFormsModule],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss',
})
export class FooterComponent implements OnInit, OnDestroy {
  model$ = combineLatest([this.appInfoService.getAppInfo(), this.authService.getCurrentUser()]).pipe(
    map(([appInfo, user]) => ({ appInfo, user })),
  );

  public loginForm!: FormGroup<{
    userName: FormControl<string | null>;
  }>;

  private subscriptions = new Subscription();

  constructor(
    private appInfoService: AppInfoService,
    private authService: AuthService,
    private browserLocalStorageService: BrowserLocalStorageService,
    private browserService: BrowserService,
    private formBuilder: FormBuilder,
  ) {}

  public ngOnInit(): void {
    this.initializeForm();
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public onSubmit() {
    const userName = this.loginForm.value.userName;
    if (userName && userName.length > 0) {
      this.subscriptions.add(
        this.authService
          .authenticate(userName)
          .pipe(
            tap((authResult) => {
              if (authResult.isAuthenticated) {
                this.browserService.redirectToPage('/list');
              }
            }),
          )
          .subscribe(),
      );
    }
  }

  public logout() {
    this.browserLocalStorageService.clear();
    this.browserService.redirectToPage('/');
  }

  private initializeForm() {
    this.loginForm = this.formBuilder.group({
      userName: this.formBuilder.control<string>(''),
    });
  }
}
