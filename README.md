# Cortado

My learning experience on Clean Archtecture, CQRS, Docker and many more ...

### How to Run

1. `cd Cortado`
2. `PS>docker-compose up`

OR

1. Open Cortado.sln file in visual studio
2. Make sure docker-compose is selected in Startup Projects dropdown
3. Click Run button

### EF Core What I did

`PM>dotnet tool install --global dotnet-ef` \
`PM>dotnet add package Microsoft.EntityFrameworkCore.Design - Did not work` \
`PM>dotnet ef migrations add initial --startup-project src/WebUI --project src/Infrastructure` \
`PM>dotnet ef database update --startup-project src/WebUI --project src/Infrastructure` \
Rollback \
`PM>dotnet ef database update <previous migration name> --startup-project src/WebUI --project src/Infrastructure`
`PM> dotnet ef migrations remove --startup-project src/WebUI --project src/Infrastructure`

#### Technologies

MediatR \
`PM>dotnet add package FluentValidation` \
Serilog.AspNetCore \
Mapster \
Mapster.DependencyInjection

### Logging with Serilog and Seq

**Important Files**

1. docker-compose.yml
2. appsettings.json
3. Program.cs
4. Any other classes that use logging

Everything created using docker-compose. Important part for Seq in compose file services name and ports

```
services:
    seq:
```

```
ports:
    - 5341:5341
    - 5342:80
```

\
**Package** \
`PM>dotnet add package Serilog.AspNetCore` \
`PM>dotnet add package Serilog.Sinks.Seq` \
`PM>dotnet add package Seq.Extensions.Logging`

**Serilog configuration**

1. In appsettings.json file, add following

```
"Serilog": {
    "Using": [
      "Serilog.Sinks.File",
      "Serilog.Sinks.Console"
    ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "/logs/log-.txt",
          "rollOnFileSizeLimit": true,
          "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter,Serilog.Formatting.Compact",
          "rollingInterval": "Day"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithThreadId", "WithMachineName" ]
}
```

**Code changes**

1. In Program.cs file, add following lines before `var app = builder.build()` line

```
builder.Host.UseSerilog((context, configuration) =>
    {
        configuration.ReadFrom.Configuration(context.Configuration)
            .WriteTo.Seq("http://**seq**:5341");
    });
```

\*\*seq: container name \ services \
2. (Optional) Log Http request, add following after `var app = builder.build()` \
`app.UseSerilogRequestLogging()`

3. To log from any class, inject `ILogger<>` class as \

```
public GetMembersQueryHandler(ILogger<GetMembersQueryHandler> logger)
{
    _logger = logger;
}
```

4. Then add following \
   `_logger.LogInformation("GetMembersQueryHandler.Handle(...) start");`

**Access Seq**

1. Type `http://localhost:5342`, ports are mapped in docker-compose.


### Patterns
Result Type/Pattern