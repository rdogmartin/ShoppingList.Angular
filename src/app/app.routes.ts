import { Routes } from '@angular/router';
import { DefaultComponent } from './layouts/default/default.component';
import { ListComponent } from './pages/list/list.component';
import { HomeComponent } from './pages/home/home.component';

export const routes: Routes = [
  {
    path: '',
    component: DefaultComponent,
    children: [
      {
        path: '',
        component: HomeComponent,
        title: 'Shopping List',
      },
      {
        path: 'list',
        component: ListComponent,
        title: 'Shopping List',
      },
    ],
  },
];
