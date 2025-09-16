# PrintBucket

## Project Layout

	|-/				root folder
    |-certs/        certificates folder
    |-doc/          documentation folder
    |-node_modules/
    |-packaging     installers folder
    |-src           source code folder    
    |-tools         multiple tools folder    
    
## Requisites

NodeJS npm 

* run npm install in root folder / npm ci
* npm install -g grunt-cli in root folder

Visual Studio .NET Framework development environment (C#)

## Packaging

### Licenses

Working folder:

    |-packaging/licenses 
        
Steps to create a license for a Printspot Site

* Create the folder structure for the Printspot site ID inside the working folder.

        |-packaging/licenses/siteID/
        |-packaging/licenses/siteID/files
        |-packaging/licenses/siteID/files/kiosk            
        |-packaging/licenses/siteID/files/ProductionCenter
        |-packaging/licenses/siteID/nsis    

#### packaging/licenses/siteID/files/kiosk

This folder contains the configuration files for the PPM folder. Create a config folder and put inside the Settings.xml encrypted file

In order to edit the Settings.xml folder you need to use tools\windows\EncryptDecryptFileTool and change the following values

    PrintSpotApiUsername
    PrintSpotApiPassword
    SiteId

*IMPORTANT: If the Settings.xml file is from another client, check the section Host username and Host Password because it includes sensible information
about the updates folder and can generate a major issue if two clients share the same update folder*

#### packaging/licenses/siteID/files/ProductionCenter

*Skip this step if the kiosk doesn't need a Production Center configuration.* 

This folder contains the configurations files for the ProductionCenter application. Copy the config.json file inside this folder

*IMPORTANT: ProductionCenter requires an encrypted services user. In order to generate an user use the tool
 tools\windows\Tools.License.Console*

#### packaging/licenses/siteID/files/nsis

Copy nsis from another folder and change the following parameters 

* NAME Pattern: "Site_ID_Config"

* OUT_DIR Pattern:  ".\..\..\deploy\{client}\licenses\" 
File will be uploaded to applications.imaxel.com/imaxel-kiosk/customers/{client}/{licenses} s3 bucket

* INSTALLATION_DIR Change to PPM (Kiosk) installation folder 
        
##Tools

###tools\windows\Tools.License.Console 

Requires two arguments:
* SiteID
* Permissions (underscore concatenaded strings)

Returns user login name for services.imaxel.com (change the text after @ to control the users that belong to an specific customer for grouping purposes) 

Usual permissions values:
* folder_printer -> Enables folder and printer output in production center.

* folder_plugin_fuji -> Enables folder and plugin output.
Fuji plugin is enabled (specific plugin permissions not yet implemented in Production Center but it's better if the user is created with the plugin permissions).

* folder -> Only folder output is enabled in production center.    

*Important: Remember to create the user in services.imaxel.com and setup the distribution rule to redirect the orders to this new user*


      

