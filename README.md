Development
===========

Visual Studio 2012 for web (free download).


Server Provisioning
===================

Over Puntdoc's _Windows Server 2008 R2 Standard SP1 installation_.

- Install MS Web Platform Installer (WPI) (we used 4.6)
    - <http://www.microsoft.com/web/downloads/platform.aspx>
- Use WPI to install
    - IIS 7 recommended configuration
    - IIS: ASP.NET
    - Run ASP.NET IIS Registration Tool: registers the handler for `.svc` files
    - IIS admin console: gives you the console application
    - IIS admin service: enables the console to manage the sites
    - WebDeploy 3.5: enables you to import Web Deploy Packages (see Deployment section below)


Deployment
==========

Using WebDeploy (not working yet)
---------------------------------

### Server requirements:

  - Run as admin in a console (Powershell, right click on icon: run as admin):
```
dism /online /enable-feature /featurename:IIS-WebServerRole
dism /online /enable-feature /featurename:IIS-WebServerManagementTools
dism /online /enable-feature /featurename:IIS-ManagementService
Reg Add HKLM\Software\Microsoft\WebManagement\Server /V EnableRemoteManagement /T REG_DWORD /D 1
net start wmsvc
sc config wmsvc start= auto
```

Using Web Deploy Package
------------------------

### Server preparation:

- In the IIS Administration software
  - Add application pool:
    - Name: `nax_web_service`
    - .Net Framework 4.0
    - Advanced settings:
      - Enable 32-bit apps
      - User: (see below)
  - Add web site with:
     - Name: `nax_web_service`
     - Application group: ASP.NET v4.0
     - Path: `C:\inetpub\wwwroot\nax_web_service` (create the directory)
- Ensure the IIS worker can access `C:\Program Files (x86)\A3` and its descendants:
  - This was tricky. One solution that worked was to ensure those files where accesible by the Admin user we had on the server (accesible via RDP) and setting the IIS worker to be run with that user. Not sure that is the best solution security wise though...

### Actual deployment:

- Right-click the web service project in VS
- On the Connection step:
  - Publish Method: Web Deploy Package
  - Package location: your preference... `.zip` package will be saved here
  - Site name: `nax_web_service`
- Upload `.zip` file to server
- In the IIS Administration software
  - Import application package (`.zip`) to the new site. Make sure you leave the access route empty, so it doesn't appear in the URL.




Resources
=========

- Deploying an Internet Information Services-Hosted WCF Service
    - <http://msdn.microsoft.com/en-us/library/aa751792(v=vs.110).aspx>
- ASP.NET IIS Registration Tool (Aspnet_regiis.exe)
    - <http://msdn.microsoft.com/en-us/library/k6h9cz8h%28VS.80%29.aspx>
    - <http://stackoverflow.com/questions/4125264/whats-the-difference-between-aspnet-regiis-ir-and-aspnet-regiis-iru>
