import { Equipo } from './equipo';

export interface MarcadorGlobal {
  equipoLocal: Equipo;
  equipoVisitante: Equipo;
  cuartoActual: number;     // 1..4
  tiempoRestante: number;   // segundos (ej. 600)
  enProrroga: boolean;
  numeroProrroga: number;
}
