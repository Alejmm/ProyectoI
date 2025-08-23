import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { MarcadorGlobal } from '../modelos/marcador-global';

@Injectable({ providedIn: 'root' })
export class MarcadorService {
  private http = inject(HttpClient);
  private readonly base = '/api/marcador';

  // ---- Lecturas ----
  obtenerMarcador(): Observable<MarcadorGlobal> {
    return this.http.get<MarcadorGlobal>(`${this.base}`);
  }

  obtenerEstadoTiempo(): Observable<{
    estado: 'Running' | 'Stopped' | 'Paused';
    cuartoActual: number;
    segundosRestantes: number;
    duracionCuarto: number;
  }> {
    return this.http.get<any>(`${this.base}/tiempo`).pipe(
      map(r => ({
        estado: r.estado,
        cuartoActual: r.cuartoActual,
        segundosRestantes: r.segundosRestantes,
        duracionCuarto: r.duracionCuarto
      }))
    );
  }

  // ---- Puntos ----
  sumarPuntos(quien: 'local'|'visitante', puntos: number): Observable<MarcadorGlobal> {
    return this.http.post<MarcadorGlobal>(
      `${this.base}/puntos/sumar?equipo=${quien}&puntos=${puntos}`, {}
    );
  }

  restarPuntos(quien: 'local'|'visitante', puntos: number): Observable<MarcadorGlobal> {
    return this.http.post<MarcadorGlobal>(
      `${this.base}/puntos/restar?equipo=${quien}&puntos=${puntos}`, {}
    );
  }

  // ---- Faltas ----
  registrarFalta(quien: 'local'|'visitante'): Observable<MarcadorGlobal> {
    return this.http.post<MarcadorGlobal>(`${this.base}/falta?equipo=${quien}`, {});
  }

  // ---- Cuartos ----
  avanzarCuarto(): Observable<MarcadorGlobal> {
    return this.http.post<MarcadorGlobal>(`${this.base}/cuarto/siguiente`, {});
  }

  // ---- Tiempo (mergeado: reloj/* y tiempo/*) ----
  iniciarReloj(): Observable<MarcadorGlobal> {
    return this.http.post<MarcadorGlobal>(`${this.base}/reloj/iniciar`, {});
  }
  
  pausarReloj(): Observable<MarcadorGlobal> {
    return this.http.post<MarcadorGlobal>(`${this.base}/reloj/pausar`, {});
  }

  reanudarTiempo(): Observable<MarcadorGlobal> {
    return this.http.post<MarcadorGlobal>(`${this.base}/tiempo/reanudar`, {});
  }

  reiniciarTiempo(segundos: number = 600): Observable<MarcadorGlobal> {
    return this.http.post<MarcadorGlobal>(`${this.base}/tiempo/reiniciar?seg=${segundos}`, {});
  }

  establecerTiempo(segundos: number): Observable<MarcadorGlobal> {
    return this.http.post<MarcadorGlobal>(`${this.base}/tiempo/establecer?seg=${segundos}`, {});
  }

  // ---- Equipos ----
  renombrarEquipos(local?: string, visitante?: string): Observable<MarcadorGlobal> {
    return this.http.post<MarcadorGlobal>(`${this.base}/equipos/renombrar`, {
      local,
      visitante
    });
  }

  nuevoPartido(): Observable<MarcadorGlobal> {
  return this.http.post<MarcadorGlobal>(`/api/marcador/nuevo`, {});
  }
}
