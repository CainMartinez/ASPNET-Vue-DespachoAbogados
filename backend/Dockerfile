# Usa la imagen oficial de .NET 9 SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia el archivo del proyecto y restaura dependencias
COPY ["AbogadosAPI.csproj", "./"]
RUN dotnet restore "AbogadosAPI.csproj"

# Copia el resto del código y compila
COPY . .
RUN dotnet build "AbogadosAPI.csproj" -c Release -o /app/build

# Publica la aplicación
FROM build AS publish
RUN dotnet publish "AbogadosAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

# Instala herramientas para esperar a que MySQL esté listo
RUN apt-get update && apt-get install -y netcat-openbsd && rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "AbogadosAPI.dll"]
