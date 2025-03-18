import { isPlatformBrowser } from '@angular/common';
import { filter, map, merge, Observable, of, ReplaySubject } from 'rxjs';
import { StorageItemKey } from '../models/enum';
import { StorageItem } from '../models/model';

export abstract class BrowserStorageService {
  private storageChanged: ReplaySubject<StorageItem> = new ReplaySubject();
  private isRenderingInBrowser: boolean;

  public constructor(
    platformId: object,
    private storageType: Storage,
  ) {
    this.isRenderingInBrowser = isPlatformBrowser(platformId);
  }

  public get<T>(key: StorageItemKey): T | null {
    if (!this.isRenderingInBrowser) {
      return {} as T;
    }
    const sessionValue = this.storageType.getItem(key);
    if (sessionValue) {
      return JSON.parse(sessionValue) as T;
    }
    return null;
  }

  public select<T>(key: StorageItemKey): Observable<T | null> {
    const valueRightNow$ = of(this.get<T>(key));
    const futureValues$ = this.storageChanged.pipe(
      filter((item) => item.key === key),
      map((item) => (item.value ? (JSON.parse(item.value) as T) : null)),
    );
    return merge(valueRightNow$, futureValues$);
  }

  public remove(key: StorageItemKey): void {
    if (!this.isRenderingInBrowser) {
      return;
    }
    this.storageType.removeItem(key);
    this.storageChanged.next({ key: key, value: '' });
  }

  public set<T>(key: StorageItemKey, value: T): void {
    if (!this.isRenderingInBrowser) {
      return;
    }
    const valueJson = JSON.stringify(value);
    this.storageType.setItem(key, valueJson);
    this.storageChanged.next({ key: key, value: valueJson });
  }

  public clear() {
    if (!this.isRenderingInBrowser) {
      return;
    }
    this.storageType.clear();
    Object.values(StorageItemKey).forEach((itemKey) => {
      this.storageChanged.next({ key: itemKey, value: '' });
    });
  }
}
