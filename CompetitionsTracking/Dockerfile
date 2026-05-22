FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CompetitionsTracking/CompetitionsTracking.csproj", "CompetitionsTracking/"]
COPY ["CompetitionsTracking.Application/CompetitionsTracking.Application.csproj", "CompetitionsTracking.Application/"]
COPY ["CompetitionsTracking.Domain/CompetitionsTracking.Domain.csproj", "CompetitionsTracking.Domain/"]
COPY ["CompetitionsTracking.Infrastructure/CompetitionsTracking.Infrastructure.csproj", "CompetitionsTracking.Infrastructure/"]
COPY ["CompetitionsTracking.Repositories/CompetitionsTracking.Repositories.csproj", "CompetitionsTracking.Repositories/"]
COPY ["CompetitionsTracking.Services/CompetitionsTracking.Services.csproj", "CompetitionsTracking.Services/"]
RUN dotnet restore "CompetitionsTracking/CompetitionsTracking.csproj"

COPY . .
WORKDIR /src/CompetitionsTracking
RUN dotnet publish "CompetitionsTracking.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CompetitionsTracking.dll"]
