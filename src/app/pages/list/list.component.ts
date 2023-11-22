import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { AuthService } from '../../shared/services/auth.service';
import { ListItemService } from '../../shared/services/listItem.service';

@Component({
  selector: 'app-list',
  standalone: true,
  imports: [CommonModule, MatCheckboxModule],
  templateUrl: './list.component.html',
  styleUrl: './list.component.scss',
})
export class ListComponent {
  currentUser = toSignal(this.authService.getCurrentUser(), {
    initialValue: { userId: '', userRoles: [], identityProvider: '', userDetails: '', claims: [] },
    requireSync: false,
  });

  listItems = toSignal(this.listItemService.getListItems(), { initialValue: [], requireSync: false });

  constructor(
    private authService: AuthService,
    private listItemService: ListItemService,
  ) {}
}
