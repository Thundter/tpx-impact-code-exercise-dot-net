# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# --- Base Stage ---
FROM ://mcr.microsoft.com AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Install NSwag tool early so it stays cached
RUN dotnet tool install -g NSwag.ConsoleCore
ENV PATH="$PATH:/root/.dotnet/tools"

# 2. Restore dependencies
COPY ["UrlShortener/UrlShortener.csproj", "UrlShortener/"]
RUN dotnet restore "./UrlShortener/UrlShortener.csproj"

# 3. Copy everything
COPY . .

# 4. Generate the C# Interface BEFORE building
# Assuming nswag.json is in the project root
WORKDIR "/src/UrlShortener"
RUN nswag run nswag.json

# 5. Build the project (now including the generated file)
RUN dotnet build "./UrlShortener.csproj" -c $BUILD_CONFIGURATION -o /app/build

# --- Publish Stage ---
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./UrlShortener.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- Final Stage ---
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UrlShortener.dll"]