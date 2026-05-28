FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY src/src.sln ./src/
COPY src/Fundo.Domain/Fundo.Domain.csproj ./src/Fundo.Domain/
COPY src/Fundo.Infrastructure/Fundo.Infrastructure.csproj ./src/Fundo.Infrastructure/
COPY src/Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj ./src/Fundo.Applications.WebApi/
COPY src/Fundo.Services.Tests/Fundo.Services.Tests.csproj ./src/Fundo.Services.Tests/

RUN dotnet restore src/src.sln

COPY src/ ./src/

RUN dotnet publish src/Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj \
    -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Fundo.Applications.WebApi.dll"]
