FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 8080
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BgituGrades/BgituGrades.API.csproj", "BgituGrades/"]
COPY ["BgituGrades.Application/BgituGrades.Application.csproj", "BgituGrades.Application/"]
COPY ["BgituGrades.Infrastructure/BgituGrades.Infrastructure.csproj", "BgituGrades.Infrastructure/"]
COPY ["BgituGrades.Domain/BgituGrades.Domain.csproj", "BgituGrades.Domain/"]

RUN dotnet restore "BgituGrades/BgituGrades.API.csproj"

COPY . .

RUN dotnet publish "BgituGrades/BgituGrades.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BgituGrades.API.dll"]