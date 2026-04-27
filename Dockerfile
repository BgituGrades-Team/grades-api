FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS loader-build # Собираем загрузчик
WORKDIR /src
COPY ["BgituGradesLoader/BgituGradesLoader.csproj", "."]
RUN dotnet restore "./BgituGradesLoader.csproj"
COPY BgituGradesLoader/ .
RUN dotnet publish "./BgituGradesLoader.csproj" \
    -c Release \
    -r linux-musl-x64 \
    --self-contained true \
    -o /app/loader-publish

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build # Собираем API
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["BgituGrades/BgituGrades.API.csproj", "BgituGrades/"]
COPY ["BgituGrades.Application/BgituGrades.Application.csproj", "BgituGrades.Application/"]
COPY ["BgituGrades.Infrastructure/BgituGrades.Infrastructure.csproj", "BgituGrades.Infrastructure/"]
COPY ["BgituGrades.Domain/BgituGrades.Domain.csproj", "BgituGrades.Domain/"]

RUN dotnet restore "BgituGrades/BgituGrades.API.csproj"

COPY . .

RUN dotnet publish "BgituGrades/BgituGrades.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final # Совмещаем в финальный образ
RUN apk add --no-cache libstdc++ gcompat icu-libs   # зависимости loader-а
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
COPY --from=loader-build /app/loader-publish ./loader 
USER $APP_UID
ENTRYPOINT ["dotnet", "BgituGrades.API.dll"]