using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System;

var builder = WebApplication.CreateBuilder(args);

// ...

var app = builder.Build();

// ...

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