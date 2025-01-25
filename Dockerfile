FROM mcr.microsoft.com/dotnet/sdk:9.0.102-alpine3.21 AS build

WORKDIR /build

COPY . .

RUN dotnet publish ./src/DockerBackup.WebApi -o artifacts/DockerBackup.WebApi -r linux-x64 --self-contained true

FROM alpine:3.21.2 AS runtime

WORKDIR /docker-backup

COPY --from=build /build/artifacts/DockerBackup.WebApi .

ENTRYPOINT [ "DockerBackup.WebApi" ]
