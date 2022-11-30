FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Game.Infrastructure/Game.Infrastructure.csproj", "Game.Infrastructure/"]
COPY ["Game.Domain/Game.Domain.csproj", "Game.Domain/"]
COPY ["Game.API/Game.API.csproj", "Game.API/"]
COPY ["Tests/Test.Game.API/Test.Game.API.csproj", "Tests/Test.Game.API/"]
COPY ["Tests/Test.Game.Domain/Test.Game.Domain.csproj", "Tests/Test.Game.Domain/"]

COPY "RpslsGame.sln" .
RUN dotnet restore "RpslsGame.sln"
COPY . .

WORKDIR "/src/Game.API"
RUN dotnet build "Game.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Game.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Game.API.dll"]
