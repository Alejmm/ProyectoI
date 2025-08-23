import { Component, OnDestroy, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { interval, Subscription } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MarcadorService } from '../servicios/marcador.service';

interface Equipo { nombre: string; puntos: number; faltas: number; }
interface MarcadorGlobal {
  equipoLocal: Equipo; equipoVisitante: Equipo;
  cuartoActual: number; tiempoRestante: number;
  enProrroga: boolean; numeroProrroga: number; relojCorriendo?: boolean;
}

@Component({
  selector: 'app-publico',
  standalone: true,
  imports: [CommonModule, MatCardModule],
  templateUrl: './tablero-publico.component.html',
  styleUrls: ['./tablero-publico.component.css']
})
export class PublicoComponent implements OnInit, OnDestroy {
  private svc = inject(MarcadorService);

  private sync$?: Subscription;
  private tick$?: Subscription;

  // estado UI
  private _d   = signal<MarcadorGlobal | null>(null);
  private _seg = signal<number>(600);

  // para buzzer robusto
  private wasRunning = false;
  private lastServerSeg = 600;
  private audioUnlocked = false;

  d() { return this._d(); }
  t() { return this._seg(); }

  // logos opcionales (coloca archivos en src/assets/)
  logoLocal  = 'assets/logo-local1.png';
  logoVisita = 'assets/logo-visitante.png';

  ngOnInit(): void {
    // desbloqueo de audio por primer gesto
    const handler = () => this.unlockAudio();
    window.addEventListener('pointerdown', handler, { once: true });
    window.addEventListener('keydown', handler, { once: true });

    // poll 1s
    this.sync$ = interval(1000).subscribe(() => {
      this.svc.obtenerMarcador().subscribe((m: MarcadorGlobal) => {
        const prevServer = this.lastServerSeg;
        this.lastServerSeg = m.tiempoRestante ?? 0;

        // caso A: servidor en 0 tras venir corriendo
        if (this.wasRunning && m.relojCorriendo === false && this.lastServerSeg === 0) {
          this.playBuzzer();
        }
        // caso B: salto de 1→valor alto (cambio de periodo)
        if (this.wasRunning && prevServer <= 1 && this.lastServerSeg > 1) {
          this.playBuzzer();
        }

        this._d.set(m);
        this._seg.set(this.lastServerSeg);

        if (m.relojCorriendo) this.startTick(); else this.stopTick();
        this.wasRunning = !!m.relojCorriendo;
      });
    });
  }

  ngOnDestroy(): void {
    this.stopTick();
    this.sync$?.unsubscribe();
  }

  private startTick() {
    if (this.tick$) return;
    this.tick$ = interval(1000).subscribe(() => {
      const s = this._seg();
      if (s > 0) {
        const next = s - 1;
        this._seg.set(next);
        if (next === 0) this.playBuzzer(); // caso C: cruce local a 0
      }
    });
  }

  private stopTick() {
    this.tick$?.unsubscribe();
    this.tick$ = undefined;
  }

  mmss(seg: number) {
    const s = Math.max(0, seg | 0), m = (s / 60) | 0, r = s % 60;
    return `${m.toString().padStart(2,'0')}:${r.toString().padStart(2,'0')}`;
  }

  unlockAudio() {
    if (this.audioUnlocked) return;
    const el = document.getElementById('buzzer') as HTMLAudioElement | null;
    el?.play().then(() => {
      el.pause(); el.currentTime = 0; this.audioUnlocked = true;
    }).catch(() => {});
  }

  private playBuzzer() {
    if (!this.audioUnlocked) return;
    const el = document.getElementById('buzzer') as HTMLAudioElement | null;
    el?.play().catch(() => {});
  }
}
