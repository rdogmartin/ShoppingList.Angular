import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { ListItem, ListItemAdd, ListItemUpdate, ListItemViewModel, UserListItems } from '../models/model';

@Injectable({
  providedIn: 'root',
})
export class ListItemService {
  constructor(private http: HttpClient) {}

  public getListItemViewModels(): Observable<ListItemViewModel[]> {
    return this.getListItems().pipe(map((listItems) => this.convertToListItemViewModels(listItems)));
  }

  public getListItems() {
    return this.http.get<UserListItems>('/api/GetListItems').pipe(map((userListItems) => userListItems.listItems));
  }

  public addItem(item: ListItemAdd) {
    return this.http
      .post<UserListItems>('/api/AddListItem', item)
      .pipe(map((userListItems) => this.convertToListItemViewModels(userListItems.listItems)));
  }

  public updateItem(item: ListItemUpdate) {
    return this.http
      .put<UserListItems>('/api/UpdateListItem', item)
      .pipe(map((userListItems) => this.convertToListItemViewModels(userListItems.listItems)));
  }

  public deleteItem(item: ListItemViewModel) {
    return this.http
      .delete<UserListItems>(`/api/DeleteListItem?item=${encodeURIComponent(item.description)}`)
      .pipe(map((userListItems) => this.convertToListItemViewModels(userListItems.listItems)));
  }

  private convertToListItemViewModels(listItems: ListItem[]) {
    return listItems.map((listItem) => {
      return {
        ...listItem,
        isBeingEdited: false,
        newDescription: '',
      } as ListItemViewModel;
    });
  }
}
