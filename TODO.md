## TODO
* PrintCenter: validacion de pedidos por regla de impresion y general.
* PrintCenter: worklfows tener backups / activos desactivados / manuales / automaticos
* Printcenter: backup de configuracion en S3 como los logs
* Station: Software downloader -> canal de updates
* PrintCenter: procesado en blanco al finalizar pedidos
* PrintCenter: visualizacion de errores si la impresión de ticket falla
* PrintCenter: horarios de funcionamiento (activar la cola de descarga durante x horas)
* PrintCenter: lanzamineto manual de pedidos
* PrintCenter: renombrar plugins Fuji MS Fuji DX/DE
* PrintCenter: poder reprocesar pedidos de services (igual no tiene sentido, solo almacen local)
* Station: generacion de QRs y visualizacion
* Station: Backend dashboard, mejorar UI 
* PrintCenter: reporting de pedidos offline a services (hay endpoint)
* Protect report device status with api key
* IsSmallScreen - Windows installation for small screens is not verified
* Instalador VC++
* Repo libvips para HEIC: https://github.com/libvips/build-win64-mxe/releases
* IMPORTANTE: PrintCenter: impresion PDF validar en equipo Fuji sobre EPSON
* IMPORTANTE: Pedidos procesandose impedir cierre [**ENCURSO_VALIDAR_Y_CERRAR_09/24r
* PrintCenter: Fuji PUD aproximacion
* PrintCenter: Informacion de printer en metodo printer
* PrintCenter: Printed
* PrintCenter: añadir buscador por fecha
* PrintCenter: Inputs David Ortiz. Reprocesar permitir multiple.
* PrintCenter: Inputs David Ortiz. Imprimir hoja de produccion o no al reprocesar
* Station: Dialogo impresora errores no sale a veces en la home
* Station: Dialogo impresión (que foto utiliza si no hay digitalsignage)
* Station: Dialogo impresion que aparezca cuando PrintCenter imprimiendo.
* Station: instalar como usuario no como administrador
* PrintCenter: en los outputfilepattern mostrar ejemplos de nombres de fichero.
* PrintCenter: Noritsu, añadir buscador y mejorar UI para que sea como la de plugin Fuji
* PrintCenter: add DownloadTime 
* Station: Fotocarnet en app  
* Station: no reporta registros si no hay modo kiosco
* PrintCenter: backups ficheros de configuracion plugins
* PrintCenter: campos de buscador en reglas de impresion (combo codigo de producto, codigo variante, campo libre, etc...)
* PrintCenter: Plugin MEU, medidas productos aproximadas?
* PrintCenter: optimizar procesado de pedidos utilizando operacion simplificada de la descarga de pedidos en services (endpoint simplified)
* PrintCenter: backup plugins
* PrintCenter: Plugin DNP -> Tamaños 9x13 13x18 en bobina 10x15, detectar papel y incluir DS820
* PrintCenter: controlar play stop que no genere multithreads de procesado
* PrintCenter: DNP -> Cutter mode a off si no hay cutter que hacer antes de empezar a imprimir
* PrintCenter: En informacion de pedido al seleccionar fila mostramos el numero del job y el nombre del producto. Mostrar el numero de unidades tipo services
* Station: corrupcion settings.xml caso MiFoto - Revisar correccion MEU
* PrintCenter: al estar procesando y cambiar a configuracion puede llegar a petar y se queda el programa petado.
* PrintCenter: procesado de pedidos PrinterTools en plugin.
* PrintCenter: Citizen / DNP -> Ampliar info problemas bitmap https://imaxel.zendesk.com/agent/tickets/12278
* Station: cargar selector de idiomas si la configuracion cambia. Que no requiera reinicio
* 
* [**DONE 05/24**] PrintCenter: modo laboratorio para Fuji + impresión Fuji.PUD en remoto (Wonder, Casanova, etc..)
* [**DONE 06/24**] PrintCenter: informa de los diferentes estados de impresión en el detalle de pedido y si el procesamiento se ha realizado correctamente.
* [**DONE 06/24**] PrintCenter: Impresión PDF en driver Impresora. Conversion de PDF a Raster y envio a impresora sin RIP.
* [**DONE 06/24**] Station: sistema de logs remotos para depuracion en bucket imaxel-applications/imaxel-kiosk/logs (se mantienen logs de 15 dias)
* [**DONE 07/24**] PrintCenter: procesado de impresiones utilizando concepto PrintJobs para agrupar ficheros y cantidad para poder permitir modelo autoservicio y control granular de impresion.
* [**DONE 07/24**] PrintCenter: diferentes salidas por carpeta, nombres  en modo kiosco solo PrintCenter 
* [**DONE 07/24**] PrintCenter: plugin Fuji.PUD para instalaciones PUD instaladas con carpetas compartidas 
* [**DONE 07/24**] PrintCenter: autoarranque en modo kiosco solo PrintCenter 
* [**DONE 07/24**] PrintCenter: Inputs David Ortiz Wonderphotoshop. Rediseño plugin Fuji (Maximizar, buscador, etc...)
* [**DONE 07/24**] PrintCenter: Añadimos Ram Usage CPU usage y Free space para detectar problemas de rendimiento en equipo en el lateral de UI PrintCenter
* [**DONE 07/24**] PrintCenter: Selector de job activado en primer job al cargar informacion de pedido
* [**DONE 07/24**] Station: Implementación comunicacion con api services.api.imaxel.com
* [**DONE 07/24**] Station: Implementación comunicacion con api printspot.api.imaxel.com
* [**DONE 08/24**] PrintCenter: Fuji-Wonder Fecha procesado en blanco a veces.
* [**DONE 08/24**] PrintCenter: Fuji-Wonder Columnas fecha procesado fecha creacion permiten ordenado por fecha
* [**DONE 08/24**] PrintCenter: corregido error procesamiento productos locales cuando hay mas de 1 job.
* [**DONE 08/24**] PrintCenter: DNP Impresion 10x15 1 unidad unicamente. A peticion de MiFoto
* [**DONE 08/24**] PrintCenter: Dialogo impresion ticket html. Para mostrar el progreso impresión hoja de trabajo
* [**DONE 09/24**] PrintCenter: Comunicacion services.api.imaxel.com. Listado de productos utiliza endpoint "ligero" para no descargar todos los datos de los productos y no superar los 10 MB.
* [**DONE 09/24**] PrintCenter: añadido backup local en carpeta printcenter/data/backup de la base de datos y el fichero de configuracion
* [**DONE 09/24**] PrintCenter: Problema procesamiento medidas en articulos simplephotobook2 con cover.
* [**DONE 09/24**] PrintCenter: Añadida exportación productos csv / importación productos de csv de las reglas de impresion
* [**DONE 09/24**] PrintCenter: Correccion problemas descarga pedidos generados desde api sandbox
* [**DONE 09/24**] PrintCenter: Mejoras UI reglas de impresion
* [**DONE 09/24**] PrintCenter: Para evitar procesado de pedidos parcial se notifica y se impide el cierre programa o pasar a la configuración cuando se esta procesando algun pedido.
* [**DONE 09/24**] PrintCenter: UI mas información sobre el código de producto código de variante en el detalle de pedido / listado de pedidos.
* [**DONE 09/24**] PrintCenter: impresión diferida hoja de producción
* [**DONE 09/24**] PrintCenter: se impide cierre del programa y cambio a la configuracion si los pedidos estan procesandose.


