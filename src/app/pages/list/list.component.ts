import { CommonModule } from '@angular/common';
import { Component, OnDestroy } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Subscription, tap } from 'rxjs';
import { ListItem } from '../../shared/models/model';
import { AuthService } from '../../shared/services/auth.service';
import { ListItemService } from '../../shared/services/listItem.service';

@Component({
  selector: 'app-list',
  standalone: true,
  imports: [CommonModule, MatCheckboxModule, MatFormFieldModule, MatInputModule, MatProgressSpinnerModule],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss',
})
export class ListComponent implements OnDestroy {
  public currentUser = toSignal(this.authService.getCurrentUser(), {
    initialValue: { userId: '', userRoles: [], identityProvider: '', userDetails: '', claims: [] },
    requireSync: false,
  });

  public userListItems = toSignal(this.listItemService.getListItems(), {
    initialValue: null,
    requireSync: false,
  });

  private subscriptions = new Subscription();

  constructor(
    private authService: AuthService,
    private listItemService: ListItemService,
  ) {}

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public onAddItemBlur(e: FocusEvent) {
    const description = (e.target as HTMLInputElement).value;

    if (description.length > 0) {
      this.addItem({ description, isComplete: false });
    }
  }

  public onCheckboxChange(event: MatCheckboxChange, updatedItem: ListItem) {
    updatedItem.isComplete = event.checked;
    this.updateItem(updatedItem);
  }

  private addItem(item: ListItem) {
    const subscription = this.listItemService
      .addItem(item)
      .pipe(
        tap(() => {
          this.userListItems()!.unshift(item);
        }),
      )
      .subscribe();

    this.subscriptions.add(subscription);
  }

  private updateItem(itemToUpdate: ListItem) {
    const subscription = this.listItemService
      .updateItem(itemToUpdate)
      .pipe(
        tap(() => {
          const index = this.userListItems()!.findIndex((item) => item.description === itemToUpdate.description);
          if (index !== -1) {
            const item = this.userListItems()!.splice(index, 1)[0];
            this.userListItems()!.push(item);
          }
        }),
      )
      .subscribe();

    this.subscriptions.add(subscription);
  }
}
