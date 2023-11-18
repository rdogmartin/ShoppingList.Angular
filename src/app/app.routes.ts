import { Routes } from '@angular/router';
import { DefaultComponent } from './layouts/default/default.component';
import { ListComponent } from './pages/list/list.component';
import { HomeComponent } from './pages/home/home.component';
import { authGuard } from './shared/guards/auth.guard';
import { noAuthGuard } from './shared/guards/no-auth.guard';

export const routes: Routes = [
  {
    path: '',
    component: DefaultComponent,
    children: [
      {
        path: '',
        component: HomeComponent,
        canActivate: [noAuthGuard],
        title: 'Shopping List',
      },
      {
        path: 'list',
        component: ListComponent,
        canActivate: [authGuard],
        title: 'Shopping List',
      },
    ],
  },
];
