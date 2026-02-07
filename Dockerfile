# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["src/MyApi/MyApi.csproj", "src/MyApi/"]
RUN dotnet restore "src/MyApi/MyApi.csproj"

# Copy everything else and publish
COPY . .
WORKDIR "/src/src/MyApi"
RUN dotnet publish "MyApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
# Copy published app and entrypoint
COPY --from=build /app/publish .
COPY src/MyApi/entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh
# Let the platform pass HTTP_PORTS (e.g. Coolify) and let the entrypoint set ASPNETCORE_URLS at runtime
# Expose a generic service port of 8080 (matches Coolify default)
EXPOSE 8080
ENTRYPOINT ["/bin/sh", "/app/entrypoint.sh"]
