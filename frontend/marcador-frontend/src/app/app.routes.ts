import { Routes } from '@angular/router';
import { TableroComponent } from './caracteristicas/tablero.component';

export const routes: Routes = [
  { path: '', component: TableroComponent },
  { path: '**', redirectTo: '' }
];
