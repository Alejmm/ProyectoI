import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app';  // <-- tu archivo se llama app.ts, exporta AppComponent

bootstrapApplication(AppComponent, appConfig)
  .catch(err => console.error(err));
