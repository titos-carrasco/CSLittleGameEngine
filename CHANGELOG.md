# Changelog

## v0.7.0 2022-06-23

- Los sonidos están disponibles sólo en Windows a través de NAudio

## 2022-06-17

- Mueve todo el manejo de sonido a librería NAudio
- Habilita sonidos en demos
- Mueve especificación de directorio de recursos como parámentro en la línea de comando

## v0.6.1 2022-06-11

- Reemplaza todos los build.sh por Makefile

## 2022-06-10

- Mueve TimeBeginPeriod() y TimeEndPeriod() al inicio y fin de la tarea TRun()
- Corrige error en reproducción de sonido
- Pruebas OK en Windows: Sonidos, TTF y y flipping funciona bien
- Corrige problema en Linux/Mono al realizar flipping. Las imágenes que fueron escaladas deben ser copiadas previamente a un bitmap con format Format32bppPArgb para que el flipping resulte bien
- Pendientes Linux:
  - Problemas con TTF en Linux/Mono no existen en VM debian bullseye, pero si en bookworm/
  - Sonidos cortos no se reproducen
  - No cierra bien a veces con sonido habilitado

## 2022-06-09

- Crea clase ImagesManager para el manejo de imagen
- Crea clase SoundManager para el manejo de sonidos
- Crea clase Fontmanager para el manejo de fonts
- Manejo de imágenes, sonido y fonts se llevan a las clases señaladas
- Canvas agrega método para mostrar imágenes precargadas
- Se corrigen los demos acorde a lo anterior
- Pendientes Linux:
  - truetype Fonts no se visualizan
  - Sonidos cortos no se reproducen
  - Flipping de imágenes
  - No cierra bien a veces con sonido habilitado
- Pendientes en Windows:
  - Probar todos los cambios

## 2022-06-07

- Corrige Refresh en Linux
- Modifica, nuevamente, mecanismo para especificar el tipo de espera para completar 1/FPS en el game loop. Para ello reduce a 1ms la resolución del clock de Windows en el momento de la espera
- Ajusta los demos acorde a lo anterior

## v0.5.4 2022-06-06

- Agrega sonido
- Modifica mecanismo para especificar el tipo de espera para completar 1/FPS en el game loop

## v0.5.3 2022-06-05

- Agrega atributo para especificar si se esperará a completar en cada cuadro el tiempo 1/FPS utilizando una espera ocupada o Sleep()
- Se actualizan los demos acorde a lo anterior
- Agrega documentación

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
