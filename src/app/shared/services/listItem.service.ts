import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ListItem } from '../models/model';

@Injectable({
  providedIn: 'root',
})
export class ListItemService {
  constructor(private http: HttpClient) {}

  public getListItems() {
    return this.http.get<ListItem[]>('/api/GetListItems');
  }
}
