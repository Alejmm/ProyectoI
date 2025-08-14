# --- Build stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app /p:UseAppHost=false

# --- Run stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Opcional: variable para escuchar en el puerto 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Copia los archivos publicados
COPY --from=build /app ./

# Exponer el puerto HTTP
EXPOSE 8080

# Entry point
ENTRYPOINT ["dotnet", "Marcador.Api.dll"]
