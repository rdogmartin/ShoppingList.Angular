import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { AppInfo } from '../../models/model';
import { toSignal } from '@angular/core/rxjs-interop';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss',
})
export class FooterComponent {
  appInfo = toSignal(this.getAppInfo());

  constructor(private http: HttpClient) {}

  private getAppInfo() {
    return this.http.get<AppInfo>('/api/AppInfo');
  }
}
