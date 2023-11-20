import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppInfo } from '../models/model';

@Injectable({
  providedIn: 'root',
})
export class AppInfoService {
  constructor(private http: HttpClient) {}

  public getAppInfo() {
    return this.http.get<AppInfo>('/api/AppInfo');
  }
}
