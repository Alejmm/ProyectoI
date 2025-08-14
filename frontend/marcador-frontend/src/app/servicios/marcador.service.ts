import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MarcadorGlobal } from '../modelos/marcador-global';

// Ajusta esta URL a donde expongas la API en desarrollo:
const API_BASE = 'http://127.0.0.1:5000/api'; // o http://IP_DE_TU_VPS:5000/api

type LadoEquipo = 'local' | 'visitante';

@Injectable({ providedIn: 'root' })
export class MarcadorService {
  constructor(private http: HttpClient) {}

  obtenerMarcador(): Observable<MarcadorGlobal> {
    return this.http.get<MarcadorGlobal>(`${API_BASE}/marcador`);
  }

  sumarPuntos(equipo: LadoEquipo, puntos: number): Observable<MarcadorGlobal> {
    const params = new HttpParams().set('equipo', equipo).set('puntos', puntos);
    return this.http.post<MarcadorGlobal>(`${API_BASE}/marcador/puntos/sumar`, null, { params });
  }

  restarPuntos(equipo: LadoEquipo, puntos: number): Observable<MarcadorGlobal> {
    const params = new HttpParams().set('equipo', equipo).set('puntos', puntos);
    return this.http.post<MarcadorGlobal>(`${API_BASE}/marcador/puntos/restar`, null, { params });
  }

  avanzarCuarto(): Observable<MarcadorGlobal> {
    return this.http.post<MarcadorGlobal>(`${API_BASE}/marcador/cuarto/siguiente`, null);
  }

  registrarFalta(equipo: LadoEquipo): Observable<MarcadorGlobal> {
    const params = new HttpParams().set('equipo', equipo);
    return this.http.post<MarcadorGlobal>(`${API_BASE}/marcador/falta`, null, { params });
  }
  establecerTiempo(segundos: number): Observable<MarcadorGlobal> {
  const params = new HttpParams().set('seg', segundos);
  return this.http.post<MarcadorGlobal>(`${API_BASE}/marcador/tiempo/establecer`, null, { params });
}

reiniciarTiempo(segundos = 600): Observable<MarcadorGlobal> {
  const params = new HttpParams().set('seg', segundos);
  return this.http.post<MarcadorGlobal>(`${API_BASE}/marcador/tiempo/reiniciar`, null, { params });
}

}
