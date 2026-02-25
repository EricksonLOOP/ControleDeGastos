FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CGD.API/CGD.API.csproj", "CGD.API/"]
COPY ["CGD.CrossCutting/CGD.CrossCutting.csproj", "CGD.CrossCutting/"]
COPY ["CGD.Domain/CGD.Domain.csproj", "CGD.Domain/"]
COPY ["CGD.Infra/CGD.Infra.csproj", "CGD.Infra/"]
RUN dotnet restore "CGD.API/CGD.API.csproj"
COPY . .
WORKDIR "/src/CGD.API"
RUN dotnet build "./CGD.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CGD.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CGD.API.dll"]
