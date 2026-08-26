# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore src/FullHost/FullHost.csproj

RUN dotnet publish src/FullHost/FullHost.csproj \
    -c Release \
    -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FullHost.dll"]