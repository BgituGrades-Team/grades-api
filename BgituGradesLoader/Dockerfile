FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
COPY ["BgituGradesLoader.csproj", "."]
RUN dotnet restore "./BgituGradesLoader.csproj"
COPY . .
RUN dotnet publish "./BgituGradesLoader.csproj" \
    -c Release \
    -r linux-musl-x64 \
    --self-contained true \
    -o /app/publish

FROM alpine:latest
RUN apk add --no-cache libstdc++ gcompat icu-libs
WORKDIR /app
COPY --from=build /app/publish ./internal_publish