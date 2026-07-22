# ── Etapa 1: build da aplicação ──
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto para restaurar as dependências
COPY ["src/FinFlow.API/FinFlow.API.csproj", "src/FinFlow.API/"]
COPY ["src/FinFlow.Application/FinFlow.Application.csproj", "src/FinFlow.Application/"]
COPY ["src/FinFlow.Domain/FinFlow.Domain.csproj", "src/FinFlow.Domain/"]
COPY ["src/FinFlow.Infra/FinFlow.Infra.csproj", "src/FinFlow.Infra/"]

RUN dotnet restore "src/FinFlow.API/FinFlow.API.csproj"

# Copia o restante do código e publica a aplicação
COPY . .
WORKDIR "/src/src/FinFlow.API"
RUN dotnet publish "FinFlow.API.csproj" -c Release -o /app/publish --no-restore

# ── Etapa 2: imagem final de runtime (menor e mais segura) ──
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FinFlow.API.dll"]
