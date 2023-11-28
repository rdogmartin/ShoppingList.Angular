import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { UserListItems } from '../models/model';

@Injectable({
  providedIn: 'root',
})
export class ListItemService {
  constructor(private http: HttpClient) {}

  public getListItems() {
    return this.http.get<UserListItems>('/api/GetListItems').pipe(map((userListItems) => userListItems.listItems));
  }
}
