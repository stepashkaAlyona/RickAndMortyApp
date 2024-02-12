import { CharListComponent } from './character/char-list/char-list.component';
import { Routes } from '@angular/router';

export const routes: Routes = [
    {path: '', redirectTo: 'char/list', pathMatch: 'full'},
    {path: 'char/list', component:CharListComponent}
];
