import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar'; 
import { MarcadorService } from '../servicios/marcador.service';
import { MarcadorGlobal } from '../modelos/marcador-global';
import { interval, Subscription } from 'rxjs';       
import { MatBadgeModule } from '@angular/material/badge';       

@Component({
  selector: 'app-tablero',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, MatSnackBarModule, MatBadgeModule], 
  templateUrl: './tablero.component.html',
  styleUrls: ['./tablero.component.css']
})
export class TableroComponent implements OnInit, OnDestroy { 
  datos = signal<MarcadorGlobal | null>(null);
  cargando = signal<boolean>(true);
  error = signal<string | null>(null);
  esperando = signal<boolean>(false);

  //  estado del reloj
  corriendo = signal<boolean>(false);       // ¿está contando?
  private reloj$?: Subscription;            // sub al interval
  private readonly MAX_TIEMPO_SEG = 20 * 60;

  constructor(private marcadorSrv: MarcadorService, private sb: MatSnackBar) {}
  
  ngOnInit(): void { this.cargar(); }
  ngOnDestroy(): void { this.pausar(); }    // asegura liberar interval

  private notificar(msg: string) {              
    this.sb.open(msg, 'OK', { duration: 2000 });
  }

  cargar() {
    this.cargando.set(true);
    this.error.set(null);
    this.marcadorSrv.obtenerMarcador().subscribe({
      next: d => { this.datos.set(d); this.cargando.set(false); },
      error: e => { this.error.set('No se pudo obtener el marcador'); this.cargando.set(false); console.error(e); }
    });
  }

  private ejecutar<T>(obs$: import('rxjs').Observable<T>, after?: (d: any)=>void) {
    this.esperando.set(true);
    obs$.subscribe({
      next: (d: any) => { after?.(d); this.datos.set(d); this.esperando.set(false); },
      error: e => { console.error(e); this.esperando.set(false); this.notificar('Ocurrió un error'); } // 
    });
  }

  // Puntos / faltas / cuarto 
  sumar(lado: 'local'|'visitante', pts: number) { this.ejecutar(this.marcadorSrv.sumarPuntos(lado, pts)); }
  restar(lado: 'local'|'visitante', pts: number) { this.ejecutar(this.marcadorSrv.restarPuntos(lado, pts)); }
  falta(lado: 'local'|'visitante') {
  const texto = lado === 'local' ? 'Falta Local' : 'Falta Visita';
  this.ejecutar(this.marcadorSrv.registrarFalta(lado), () => {
    this.notificar(`${texto} registrada`);
  });
}

siguienteCuarto() {
  this.pausar();
  this.ejecutar(this.marcadorSrv.avanzarCuarto(), (res) => {
    if (res.enProrroga && res.numeroProrroga === 1) {
      this.notificar('¡Prórroga! 5:00');
    } else if (res.enProrroga) {
      this.notificar(`¡Prórroga ${res.numeroProrroga}! 5:00`);
    } else {
      this.notificar(`Cuarto ${res.cuartoActual}`);
    }
  });
}

  
  iniciar() {
    if (this.corriendo() || !this.datos()) return;
    this.corriendo.set(true);
    this.reloj$ = interval(1000).subscribe(() => {
      const d = this.datos();
      if (!d) return;
      const restante = Math.max(0, d.tiempoRestante - 1);
      this.datos.set({ ...d, tiempoRestante: restante });
      if (restante === 0) this.pausar();    // se detiene al llegar a 00:00
    });
  }

  pausar() {
    this.reloj$?.unsubscribe();
    this.reloj$ = undefined;
    this.corriendo.set(false);
  }

reiniciar() {
  this.pausar();
  const d = this.datos(); if (!d) return;
  const seg = d.enProrroga ? 300 : 600;   // 5:00 si es prórroga, 10:00 si no
  this.ejecutar(this.marcadorSrv.reiniciarTiempo(seg), () => {
    this.notificar(`Tiempo reiniciado a ${this.mmss(seg)}`);
  });
}

  ajustar(segundos: number) {
    const d = this.datos(); if (!d) return;
    // clamp 0..MAX_TIEMPO_SEG
    let nuevo = d.tiempoRestante + segundos;
    if (nuevo < 0) { nuevo = 0; this.notificar('Mínimo alcanzado: 00:00'); }               
    if (nuevo > this.MAX_TIEMPO_SEG) { nuevo = this.MAX_TIEMPO_SEG; this.notificar('Máximo 20:00'); }

    this.ejecutar(this.marcadorSrv.establecerTiempo(nuevo), (res) => {
      this.notificar(`Tiempo actualizado a ${this.mmss(res.tiempoRestante)}`); 
    });
  }


  mmss(seg: number) {
    const m = Math.floor(seg / 60).toString().padStart(2,'0');
    const s = (seg % 60).toString().padStart(2,'0');
    return `${m}:${s}`;
  }
}
