FROM mcr.microsoft.com/dotnet/sdk:9.0.102-alpine3.21 AS build

WORKDIR /build

COPY . .

RUN dotnet publish ./src/DockerBackup.WebApi -o artifacts/DockerBackup.WebApi

FROM mcr.microsoft.com/dotnet/aspnet:9.0.1-alpine3.21 AS runtime

WORKDIR /docker-backup

COPY --from=build /build/artifacts/DockerBackup.WebApi .

ENTRYPOINT [ "dotnet", "DockerBackup.WebApi.dll" ]
