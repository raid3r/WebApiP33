# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY WebApiP33/WebApiP33.csproj WebApiP33/
RUN dotnet restore WebApiP33/WebApiP33.csproj

COPY WebApiP33/ WebApiP33/
RUN dotnet publish WebApiP33/WebApiP33.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "WebApiP33.dll"]
