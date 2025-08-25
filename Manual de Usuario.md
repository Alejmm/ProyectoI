# 📖 Manual de Usuario
# https://github.com/Alejmm/ProyectoI 
## Creadores: Jenny Sofia Morales López 7690 08 6790 y Cristian Alejandro Melgar Ordoñez 7690 21 8342
# UMG

## 1. Introducción
El presente manual tiene como objetivo guiar al usuario en el uso del **Marcador de Baloncesto en Tiempo Real**, una aplicación web que permite llevar el control de puntos, tiempo de juego, cuartos y faltas de manera intuitiva.  

La aplicación también incluye la opción de **guardar partidos** para mantener un registro histórico.

---

## 2. Acceso a la Aplicación
1. Abre tu navegador web (Chrome, Edge, Firefox, Safari).  
2. Ingresa la dirección proporcionada por el administrador del sistema, por ejemplo:  
   👉 `http://localhost:4200` (modo local).  
3. Se mostrará la **pantalla principal del marcador**.  

---

## 3. Pantalla Principal
La interfaz se divide en las siguientes secciones:

- **Marcador de equipos:** muestra el puntaje actual de cada equipo.  
- **Temporizador:** cronómetro configurable para cada cuarto.  
- **Cuarto actual:** indica si es 1Q, 2Q, 3Q o 4Q.  
- **Faltas:** contador de faltas por equipo.  
- **Controles:** botones de suma/resta de puntos, manejo del tiempo y reinicio.  

- **Funciones**
- Renombrar equipos.
- Vista pública.
- Inicio de partido.
- Reiniciar partido.
- Registro de faltas 
- Autoavanzar al siguiente cuarto
- Nuevo partido

- Tablero de control en modo pantalla desktop
![Imagen de WhatsApp 2025-08-24 a las 21 56 00_6ca2eebf](https://github.com/user-attachments/assets/5b3ee5d2-1415-4705-8d57-3795a4c4f278)

- Tablero de control en modo pantalla móvil
![Imagen de WhatsApp 2025-08-23 a las 17 41 52_25126517](https://github.com/user-attachments/assets/a871f731-ee30-45b5-81a2-9a7ac1303100)

- Vista al publico
![Imagen de WhatsApp 2025-08-24 a las 21 56 16_35075f00](https://github.com/user-attachments/assets/8b487496-1232-4514-ae64-7d9b674a86f0)


- Renombrar equipos
![Imagen de WhatsApp 2025-08-24 a las 21 56 47_9603f196](https://github.com/user-attachments/assets/683fd77a-13f1-44b6-aafd-4318267babd4)



## 4. Funcionalidades Principales

### 4.1 Gestión de Puntos
- Botones **+1, +2, +3**: suman puntos al equipo seleccionado.  
- Botones de resta (−1, −2, −3): corrigen errores de puntuación.  

### 4.2 Gestión de Tiempo
- **Iniciar:** activa el temporizador del cuarto.  
- **Pausa:** detiene el tiempo temporalmente.  
- **Reiniciar:** vuelve el temporizador al valor configurado (ej. 10:00).  
- **Notificación sonora/visual**: al finalizar el cuarto.  

### 4.3 Manejo de Cuartos
- Al terminar el tiempo, se muestra aviso de **fin de cuarto**.  
- Se puede avanzar manualmente al siguiente cuarto desde los controles.  

### 4.4 Control de Faltas
- Botones para aumentar o disminuir las faltas de cada equipo.  
- El contador se resetea al iniciar un nuevo partido.  

### 4.5 Reinicio del Partido
- El botón **Reiniciar marcador** borra puntos, faltas, cuartos y tiempo, dejando todo en 0.  

---

## 5. Persistencia de Partidos

### 5.1 Guardar Partido
1. Desde la interfaz, presiona el botón **Guardar**.  
2. El sistema enviará la información al backend y almacenará:  
   - Nombre de los equipos.  
   - Puntos y faltas actuales.  
   - Cuarto en juego.  
   - Tiempo restante.  
3. Aparecerá un mensaje de confirmación:  
   👉 *"Partido guardado exitosamente"*.

---

## 6. Errores Comunes y Soluciones
| Problema | Causa probable | Solución |
|----------|----------------|----------|
| El temporizador no inicia | No se presionó **Iniciar** | Presiona el botón **Iniciar** |
| No se guardan los partidos | El backend no está ejecutándose | Verifica que el servidor Node.js esté en `http://localhost:3000` |
| Los puntos no cambian | El navegador está congelado | Refresca la página o reinicia el navegador |
| No aparece la lista de partidos guardados | La base de datos está vacía o no conectada | Verifica la conexión y crea un nuevo partido |

---

## 7. Recomendaciones de Uso
- Verificar que el navegador esté actualizado.  
- Guardar el partido antes de cerrar la aplicación para no perder los datos.  
- Reiniciar el temporizador al inicio de cada cuarto.  
- Usar los botones de corrección (resta) solo en caso de error humano.  

---

## 8. Futuras Mejoras en el Uso
- Registro de jugadores individuales y sus estadísticas.  
- Exportación de partidos en PDF/Excel.  
- Integración con pantallas LED y móviles.  
- Soporte multilenguaje (Español/Inglés).  

---

## 9. Contacto de Soporte
En caso de inconvenientes, contactar al administrador del sistema o al equipo de desarrollo mediante:  
📧 cmelgaro@miumg.edu.gt y  jmoralesl15@miumg.edu.gt
---


