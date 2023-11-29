import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs';
import { ListItem, UserListItems } from '../models/model';

@Injectable({
  providedIn: 'root',
})
export class ListItemService {
  constructor(private http: HttpClient) {}

  public getListItems() {
    return this.http.get<UserListItems>('/api/GetListItems').pipe(map((userListItems) => userListItems.listItems));
  }

  public addItem(item: ListItem) {
    return this.http.post<UserListItems>('/api/AddListItem', item);
  }

  public updateItem(item: ListItem) {
    return this.http.put<UserListItems>('/api/UpdateListItem', item);
  }
}
