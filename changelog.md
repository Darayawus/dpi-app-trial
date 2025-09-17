# imaxel-station
Imaxel Station solution

## HISTORY
### 4.12.2 (Wed Oct  4 11:58:10 UTC 2017) <jedf@imaxel.com>

### 2.2.X
- Printcenter: logs location moved to Station logs folder.
- Station: Landscape Config -> Possibility to enable "Create order with USB/Card" ... Only if Kiosk has a Printspot Profile and IsOrderTerminal=False
- Station: QR configuration
- Station: Self-Service autorestarter
- Station: Cloud config -> ScreenSaver and CommercialScreenSaver Timeout
- Station: Administration layout for portrait mode revised
- Station: Device downloader UI for portrait / landscape revised
- Station: Cloud config -> download PrintCenter remote config files
- PrintCenter: DNP Plugin -> Added publication folder. Allows publication of txt files with the status of every printer
-


### 2.1.X
    * Color correction and ICC Profiles    
    * Station Config: Collect service can be configured in station config
    * Station: Collect mode is no longer activated if ecommerce profile is found.
    * PrintCenter: Printers form now is shown in a modal dialog
    * PrintCenter: logs format changed to include order num
    * PrintCenter Noritsu - Fuji plugins: variant code fix to handle the proper channels.
    * Station: Printspot communication is now using Api printspot.api.imaxel.com

### 2.0.X
    * Release for production (lots of changes)

### 1.5.3
    * NSIS installation process review
    * Detection of net core 6
    * Installer logs  in $INSTDIR\logs\Installer.log
    * AWS Api gateway for production environment cdm.apps.imaxel.com 
    * Mensaje CheckValidLicense añadido en caso de error para mostrar por pantalla
    
### 1.5.1

- Revisión de comunicación con CDM, pantallas y acciones. La pantalla de recogida de datos actualmente no envía los datos a ningun lado.
- Activación de licencia permite formato {LICENSE_KEY} o {LICENSE_KEY}-{MODULE_ID}
- Sistema de actualización incorporado a versiones 1.5.0 / 1.5.1 (configurado bajo cuenta genérica Imaxel)
- Flujos de trabajo Imaxel Station producto FREEMIUM (apertura en modo backend)
- PrintCenter configuración por defecto, salida a carpeta + salida impresora. Sin workflows
- QuickSmartphoneDownloader UI
- Revisión de literales
- Instalador en ingles e español
- Pantalla de administración en landscape rediseñada.
- Installation path changed to %appdata%\imaxel\

### 1.5.0

- Reinicio de Print Center  (se realiza en la pantalla de "Escoge que quieres hacer", con lo cual en cada inicio de pedido se reinicia el Print Center en caso de no estar disponible)

- Logs mejorados, se ha estado revisando el proceso de logs para eliminar llamadas innecesarias a Printspot y aumentar legibilidad

- Activación y comunicación con sistema de licencias CDM

- Añadimos recogida de datos de dispositivo para comunicación posterior a API DeviceStatus

- Bug fix: Escalado de precios en printspot no se aplica bien a los productos locales

- Grabación de cookie para comunicación con Printspot de pedidos generados (se tiene que verificar)






### 1.4

- Sistema de actualización https://wiki.imaxel.com/books/kiosk/page/actualizaciones

- Tickets https://wiki.imaxel.com/books/kiosk/page/tickets

- Lectura smartphone  https://wiki.imaxel.com/books/kiosk/page/lectura-smartphone

- Productos locales  https://wiki.imaxel.com/books/kiosk/page/local-products

- Productos offline para modo sin conexión. https://wiki.imaxel.com/books/kiosk/page/offline-products

- Production Center https://wiki.imaxel.com/books/kiosk/page/production-center-2a3

- Pantalla de administración. Link directo a ScreenConnect

- Botones ocultos https://wiki.imaxel.com/books/kiosk/page/botones-ocultos

- Sistema de subscripciones

- Proceso de instalación (kiosco desinstala antiguo Production Center)

### 1.0.0 (21/04/2021) <dario@imaxel.com>
  * PRCE-21 Landscape mode for kiosk
    

https://services.imaxel.com/imaxel_dev/admin/#/

### MEU - 1.8.2

- PPMStation - Settings.xml corrupto https://app.clickup.com/t/4759490/PROJECT-462

- #6797 actualización de salvapantallas, que minimizar y que pasa cuando falla. https://app.clickup.com/t/4759490/PROJECT-253

- 1.7.1.2 Users policy. Operator to be more restricted https://app.clickup.com/t/4759490/PROJECT-88

- 1.1.3 Windows OS: lock physical keyboard https://app.clickup.com/t/4759490/PROJECT-89

### MEU - 1.8.3

https://app.clickup.com/t/4759490/PROJECT-723

- Cambiar pwd Operador HEMA (únicamente HEMA shops, 111 & 838) 1433

- Forzar desactivar teclado a TRUE tras instalación

### MEU - 1.8.4

https://app.clickup.com/t/4759490/PROJECT-744

- Aplicar powershell activación KeyFilter Windows


