# Changelog

## v0.5.2 2022-06-04

- Ajustes de compatibilidad con Mono en Linux
- Corrige error en Flip al cargar las imágenes
- Se lleve a .Net 6 en Windows

## 2022-06-04

- Optimizaciones varias para alcanzar mayores FPS (DrawImage es lento)
- Se agrega en LGE getLPS() para obtener el tiempo promedio de rpoceso en cada ciclo del loop
- getFPS() entrega ahora los FPS acorde a las invocaciones de Paint()

## 2022-06-01

- Agrega documentación en el código
- Modifica OnMainUpdate() y la convierte en variable pública eliminando IEvents
- Ajusta demos acorde a lo anterior

## 2022-05-29

- Corrige errror en la salida del Game Loop
- Formatea archivos
- Mueve algunos demos
- Se libera primer release con demos

## 2022-05-28

- Se migró todo el código de Java a C#
- Pendientes Sonidos y archivos TTF
- Revisar en Windows

- Corrige
  - Los Textos no se visualizan, sólo aparecen rectángulos => En linux hay problemas con los TTF
  - Las opciones Flip de las imágenes no están funcionando => En linux los RotateFlipType tienen problemas
  - En la ventana principal quedan unos bordes que no se refrescan
- Agrega 5to demo

- Se lleva todo a SDK4.5 y C# v6
- 4 demos operativos
- Compila y ejecuta en Linux Debian (Mono y DotNet) y Windows
- Problemas detectados:
  - Los Textos no se visualizan, sólo aparecen rectángulos
  - Las opciones Flip de las imágenes no están funcionando
  - En la ventana principal quedan unos bordes que no se refrescan

## 2022-05-26

- Conversión desde Java
