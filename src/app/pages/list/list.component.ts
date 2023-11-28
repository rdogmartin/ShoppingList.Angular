import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from '../../shared/services/auth.service';
import { ListItemService } from '../../shared/services/listItem.service';

@Component({
  selector: 'app-list',
  standalone: true,
  imports: [CommonModule, MatCheckboxModule, MatProgressSpinnerModule],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss',
})
export class ListComponent {
  currentUser = toSignal(this.authService.getCurrentUser(), {
    initialValue: { userId: '', userRoles: [], identityProvider: '', userDetails: '', claims: [] },
    requireSync: false,
  });

  userListItems = toSignal(this.listItemService.getListItems(), {
    initialValue: null,
    requireSync: false,
  });

  constructor(
    private authService: AuthService,
    private listItemService: ListItemService,
  ) {}
}
