import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MarcadorGlobal } from '../modelos/marcador-global';

// Ajusta esta URL a donde expongas la API en desarrollo:
const API_BASE = '/api/marcador'; // s/api

type LadoEquipo = 'local' | 'visitante';

@Injectable({ providedIn: 'root' })
export class MarcadorService {
  constructor(private http: HttpClient) {}

  obtenerMarcador(): Observable<MarcadorGlobal> {
    return this.http.get<MarcadorGlobal>(`${API_BASE}`);
  }

  sumarPuntos(equipo: LadoEquipo, puntos: number): Observable<MarcadorGlobal> {
    const params = new HttpParams().set('equipo', equipo).set('puntos', puntos);
    return this.http.post<MarcadorGlobal>(`${API_BASE}/puntos/sumar`, null, { params });
  }

  restarPuntos(equipo: LadoEquipo, puntos: number): Observable<MarcadorGlobal> {
    const params = new HttpParams().set('equipo', equipo).set('puntos', puntos);
    return this.http.post<MarcadorGlobal>(`${API_BASE}/puntos/restar`, null, { params });
  }

  avanzarCuarto(): Observable<MarcadorGlobal> {
    return this.http.post<MarcadorGlobal>(`${API_BASE}/cuarto/siguiente`, null);
  }

  registrarFalta(equipo: LadoEquipo): Observable<MarcadorGlobal> {
    const params = new HttpParams().set('equipo', equipo);
    return this.http.post<MarcadorGlobal>(`${API_BASE}/falta`, null, { params });
  }
  establecerTiempo(segundos: number): Observable<MarcadorGlobal> {
  const params = new HttpParams().set('seg', segundos);
  return this.http.post<MarcadorGlobal>(`${API_BASE}/tiempo/establecer`, null, { params });
}

reiniciarTiempo(segundos = 600): Observable<MarcadorGlobal> {
  const params = new HttpParams().set('seg', segundos);
  return this.http.post<MarcadorGlobal>(`${API_BASE}/tiempo/reiniciar`, null, { params });
}

}
