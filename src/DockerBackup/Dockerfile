FROM mcr.microsoft.com/dotnet/runtime:8.0.1-alpine3.19
WORKDIR /app
COPY artifacts/docker-backup .
ENTRYPOINT ["dotnet", "DockerBackup.dll"]
