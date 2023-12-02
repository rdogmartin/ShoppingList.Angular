import { A11yModule } from '@angular/cdk/a11y';
import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Subscription, tap } from 'rxjs';
import { ListItem } from '../../shared/models/model';
import { ListItemService } from '../../shared/services/listItem.service';
@Component({
  selector: 'app-list',
  standalone: true,
  imports: [
    A11yModule,
    CommonModule,
    MatButtonModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatProgressSpinnerModule,
    ReactiveFormsModule,
  ],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss',
})
export class ListComponent implements OnInit, OnDestroy {
  public userListItems = toSignal(this.listItemService.getListItems(), {
    initialValue: null,
    requireSync: false,
  });

  public listForm!: FormGroup<{
    newItem: FormControl<string | null>;
  }>;

  private subscriptions = new Subscription();

  constructor(
    private formBuilder: FormBuilder,
    private listItemService: ListItemService,
  ) {}

  public ngOnInit(): void {
    this.initializeForm();
  }

  public ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  public onSubmit() {
    const description = this.listForm.value.newItem;
    if (description && description.length > 0) {
      this.addItem({ description, isComplete: false });
    }
  }

  public onCheckboxChange(event: MatCheckboxChange, updatedItem: ListItem) {
    updatedItem.isComplete = event.checked;
    this.updateItem(updatedItem);
  }

  public saveListItem(listItem: ListItem) {
    console.log('saveListItem', listItem);
  }

  public deleteListItem(listItem: ListItem) {
    console.log('deleteListItem', listItem);
  }

  private initializeForm() {
    this.listForm = this.formBuilder.group({
      newItem: this.formBuilder.control<string>(''),
    });
  }

  private addItem(item: ListItem) {
    const subscription = this.listItemService
      .addItem(item)
      .pipe(
        tap((userListItems) => {
          this.userListItems = signal(userListItems.listItems);
          this.listForm.controls.newItem.reset();
        }),
      )
      .subscribe();

    this.subscriptions.add(subscription);
  }

  private updateItem(itemToUpdate: ListItem) {
    const subscription = this.listItemService
      .updateItem(itemToUpdate)
      .pipe(tap((userListItems) => (this.userListItems = signal(userListItems.listItems))))
      .subscribe();

    this.subscriptions.add(subscription);
  }
}
