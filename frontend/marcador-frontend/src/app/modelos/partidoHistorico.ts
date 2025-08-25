export interface PartidoHistorico {
  id: number;
  fecha: string;              // llega como ISO string en JSON
  estado: string;
  equipoLocalId: number;
  equipoVisitanteId: number;
  puntosLocal: number;
  puntosVisitante: number;
  observaciones: string;
}