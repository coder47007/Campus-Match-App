FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CampusMatch.Api/CampusMatch.Api.csproj", "CampusMatch.Api/"]
COPY ["CampusMatch.Shared/CampusMatch.Shared.csproj", "CampusMatch.Shared/"]
RUN dotnet restore "CampusMatch.Api/CampusMatch.Api.csproj"
COPY . .
WORKDIR "/src/CampusMatch.Api"
RUN dotnet build "CampusMatch.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CampusMatch.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "CampusMatch.Api.dll"]
