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
import { Subscription, finalize, tap } from 'rxjs';
import { ListItemAdd, ListItemUpdate, ListItemViewModel } from '../../shared/models/model';
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
  public listItems = toSignal(this.listItemService.getListItemViewModels(), {
    initialValue: null,
    requireSync: false,
  });

  public listForm!: FormGroup<{
    newItem: FormControl<string | null>;
  }>;

  public apiCallInProgress = false;

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
      this.addItem({ description });
    }
  }

  public onCheckboxChange(event: MatCheckboxChange, listItem: ListItemViewModel) {
    listItem.isComplete = event.checked;
    listItem.isBeingEdited = true;
    this.updateItem({
      description: listItem.description,
      isComplete: listItem.isComplete,
      newDescription: listItem.newDescription,
    });
  }

  public beginEditListItem(listItem: ListItemViewModel) {
    this.listItems()?.forEach((item) => (item.isBeingEdited = false));
    listItem.isBeingEdited = true;
  }

  public onKeyUpListItem(e: Event, listItem: ListItemViewModel) {
    const description = (e.target as HTMLInputElement).value;
    listItem.newDescription = description;
  }

  public saveListItem(listItem: ListItemViewModel) {
    const subscription = this.listItemService
      .updateItem({
        description: listItem.description,
        isComplete: listItem.isComplete,
        newDescription: listItem.newDescription,
      })
      .pipe(
        tap((listItems) => {
          this.listItems = signal(listItems);
        }),
      )
      .subscribe();

    this.subscriptions.add(subscription);
  }

  public deleteListItem(listItem: ListItemViewModel) {
    const subscription = this.listItemService
      .deleteItem(listItem)
      .pipe(
        tap((listItems) => {
          this.listItems = signal(listItems);
        }),
      )
      .subscribe();

    this.subscriptions.add(subscription);
  }

  private initializeForm() {
    this.listForm = this.formBuilder.group({
      newItem: this.formBuilder.control<string>(''),
    });
  }

  private addItem(item: ListItemAdd) {
    // Insert the item in the list so that the UI updates immediately, then call the API to add it to the database.
    const newListItem: ListItemViewModel = {
      description: item.description,
      imageUrl: '',
      isComplete: false,
      isBeingEdited: true,
      newDescription: '',
    };

    this.listItems()?.unshift(newListItem);
    this.listForm.controls.newItem.reset();
    this.apiCallInProgress = true;

    const subscription = this.listItemService
      .addItem(item)
      .pipe(
        tap((listItems) => (this.listItems = signal(listItems))),
        finalize(() => (this.apiCallInProgress = false)),
      )
      .subscribe();

    this.subscriptions.add(subscription);
  }

  private updateItem(itemToUpdate: ListItemUpdate) {
    this.apiCallInProgress = true;

    const subscription = this.listItemService
      .updateItem(itemToUpdate)
      .pipe(
        tap((listItems) => (this.listItems = signal(listItems))),
        finalize(() => (this.apiCallInProgress = false)),
      )
      .subscribe();

    this.subscriptions.add(subscription);
  }
}
