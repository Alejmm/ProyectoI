import { Routes } from '@angular/router';
import { TableroComponent } from './caracteristicas/tablero.component';
import { PublicoComponent } from './publico/tablero-publico.component';  

export const routes: Routes = [
  { path: '', redirectTo: 'control', pathMatch: 'full' },
  { path: 'control', component: TableroComponent },  // operador
  { path: 'publico', component: PublicoComponent },  // Tablero-pública
  { path: '**', redirectTo: 'control' }
];
