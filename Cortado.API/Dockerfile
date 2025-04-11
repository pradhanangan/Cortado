# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN pwd
# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Cortado.API/Cortado.API.csproj", "Cortado.API/"]
COPY ["Modules/Bookings/src/Bookings.Application/Bookings.Application.csproj", "Modules/Bookings/src/Bookings.Application/"]
COPY ["Modules/Products/src/Products.Application/Products.Application.csproj", "Modules/Products/src/Products.Application/"]
COPY ["Modules/Products/src/Products.Domain/Products.Domain.csproj", "Modules/Products/src/Products.Domain/"]
COPY ["Modules/Bookings/src/Bookings.Domain/Bookings.Domain.csproj", "Modules/Bookings/src/Bookings.Domain/"]
COPY ["Modules/Bookings/src/Bookings.Infrastructure/Bookings.Infrastructure.csproj", "Modules/Bookings/src/Bookings.Infrastructure/"]
COPY ["Modules/Customers/src/Customers.Application/Customers.Application.csproj", "Modules/Customers/src/Customers.Application/"]
COPY ["Modules/Customers/src/Customers.Domain/Customers.Domain.csproj", "Modules/Customers/src/Customers.Domain/"]
COPY ["Modules/Customers/src/Customers.Infrastructure/Customers.Infrastructure.csproj", "Modules/Customers/src/Customers.Infrastructure/"]
COPY ["Modules/Products/src/Products.Infrastructure/Products.Infrastructure.csproj", "Modules/Products/src/Products.Infrastructure/"]
COPY ["Modules/Shared/src/Shared.Common/Shared.Common.csproj", "Modules/Shared/src/Shared.Common/"]
RUN dotnet restore "./Cortado.API/Cortado.API.csproj"
COPY . .
WORKDIR "/src/Cortado.API"
RUN dotnet build "./Cortado.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Cortado.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cortado.API.dll"]