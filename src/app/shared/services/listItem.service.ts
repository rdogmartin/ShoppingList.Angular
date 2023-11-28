import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { UserListItems } from '../models/model';

@Injectable({
  providedIn: 'root',
})
export class ListItemService {
  constructor(private http: HttpClient) {}

  public getListItems() {
    return this.http.get<UserListItems>('/api/GetListItems');
  }
}
