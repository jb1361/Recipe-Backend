# CshoHoldem.Api

## Setup & Requirements
* Download and install the [.NET Core 3.1.101 SDK](https://dotnet.microsoft.com/download/dotnet-core/thank-you/sdk-3.1.101-windows-x64-installer) 
and Runtime [.NET Core 3.1.101 Runtime](https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.10-windows-x64-installer).
 If you are planning on changes the version please see [Rider Known Issues](#Rider-Known-Issues) 
* Visual Studio 2019 or Jetbrains Rider 2019.3.4. VS can be installed through [Visual Studio Installer](https://visualstudio.microsoft.com/downloads/).
* Download and install [MariaDB 10.2.13](https://downloads.mariadb.org/mariadb/10.2.13/).
    * Specify MariaDB as the service name so you can recognize it
    * Use The Task Manager -> Services tab to start / stop the service
    * add `lower_case_table_names = 2` in the my.ini file under the `[mysqld]` section. 
    Use google to find out where the my.ini is located for your OS. This is used to allow capital letters in database and table names.
* run `dotnet tool restore` in the project's root directory
* [Export the database from staging](#Export-DB-from-Staging) and import it locally using Mysql Workbench.
* run `dotnet ef database update` in the `UptimeRMX.Api` subfolder of the project. 
You will need to perform this step every time a new migration is added.

## Api Documentation
https://documenter.getpostman.com/view/4756372/SVfQR8kC?version=latest

## Useful Commands
To generate controllers based off of models, install the [code generator](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/tools/dotnet-aspnet-codegenerator?view=aspnetcore-2.2)
then run 
```
dotnet aspnet-codegenerator controller
-name ConfigurationsController 
-api
-async
-m UptimeRMX.Api.Models.Configuration
-dc DefaultContext
-namespace UptimeRMX.Api.Controllers
-outDir Controllers
```

## Rider Known Issues

* Rider 2019.3.4 fails to run unit tests on .net core 3.1.201


## Export DB from Staging

[export_db_small.sh](export_db_small.sh) is a script made to get a small copy of the latest data from the production server.
This speeds up testing quite a bit by not including archived data.

Copy `export_db_small.sh` to the a user's home directory and add a `.my.cnf` file with the username and password to the db
in the format shown below.

```
[mysqldump]
user=<username>
password=<password>
```

Execute the script using `./export_db_small.sh` and enter how far back you want data exported for.
After it is exported, use sftp to download the `small_export.sql.gz`.
