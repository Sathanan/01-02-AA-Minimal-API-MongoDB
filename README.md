# 01 & 02-AA-Minimal-API-MongoDB
---

### Description
---
- In diesem Projekt werde ich in meinem Minimal Api Grundgerüst MongoDB einbinden.

## Git 
---
- Wenn Sie git benutzen wollen können sie ein bereit erstellten git repository clonen und den Folgenden befehlen  könne Sie es einbinden.
````
git clone
dotnet new gitignore
````
## Add .Net 7 
---
- Für unser Grundgerücst braucen wir .Net 7
  Hier finden Sie die Anleitung

````
# Get Ubuntu version
declare repo_version=$(if command -v lsb_release &> /dev/null; then
lsb_release -r -s; else grep -oP '(?<=ˆVERSION_ID=).+' /etc/os-release
| tr -d '"'; fi)
# Download Microsoft signing key and repository
wget https://packages.microsoft.com/config/ubuntu/$repo_version/packagesmicrosoft-prod.deb -O
packages-microsoft-prod.deb
# Install Microsoft signing key and repository
sudo dpkg -i packages-microsoft-prod.deb
# Clean up
rm packages-microsoft-prod.deb
# Update packages
sudo apt update
# Install dotnet 7 sdk
sudo apt install dotnet-sdk-7.0
````

- mit folgendem Command überprüfen Sie, ob .NET7 erfolgreich installiert ist
````
dotnet --info
````

## Projekt erstellen
---

- Erstellen SIe den Ordnerstruktur und legen Sie ein Projekt an

````
mkdir m347-minimal-api
cd m347-minimal-api
dotnet new web --name WebApi
dotnet add package MongoDB.Driver

````

## Dockerfile
---

- Für dieses Projekt müssen wir innerhalb der Webapi file ein Dockerfile erstellen

````
touch Dockerfile
nano Dockerfile

# The following code should be in the Dockerfile

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o out
COPY --from=build /app/out .
EXPOSE 5001
ENTRYPOINT ["dotnet", "Minimal-API.dll"]
````

## Docker-compose
---

- Nun erstellen wir einen docker-compose.yml file ausserhalb unserer Weapi file und geben folgenden code in dem file


````
version: '3'
services:
  webapi:
    build:
      context: ./minimal-api-with-mongodb/WebApi
      dockerfile: Dockerfile
    ports:
      - 5001:80
    depends_on:
      - mongodb
    environment:
      - MoviesDatabaseSettings__ConnectionString=mongodb://gbs:geheim@mongodb:27017
  mongodb:
    image: mongo
    restart: always
    ports:
      - 27017:27017
    volumes:
      - mongodb_data:/data/db
volumes:
  mongodb_data:
````

## Programm.cs
---

- Nun bearbeiten wir unser Programm.cs file wie folgt:

````
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => {
    return "Minimal API Version 1.0";
});

app.MapGet("/check", () =>
{
    try
    {
        var mongoClient = new MongoClient("mongodb://gbs:geheim@localhost:27017");
        var databases = mongoClient.ListDatabases().ToList();

        string response = "Zugriff auf MongoDB OK.\n\nDatenbanken:\n";
        foreach (var db in databases)
        {
            response += $"{db["name"]}\n";
        }

        return response;
    }
    catch (Exception ex)
    {
        return $"Fehler beim Zugriff auf MongoDB: {ex.Message}";
    }
});

app.Run();
````

## Final
---

- Um unser WebApi zu starten geben Sie folgende Befehl im Terminal

````
docker-compose up
````

 ### Nun können Sie unsere Webseite unter "localhost:5001" aufrufen.
