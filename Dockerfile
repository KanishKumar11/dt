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
ENV ASPNETCORE_URLS="http://+:80"
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "MyApi.dll"]
