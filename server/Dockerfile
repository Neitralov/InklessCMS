FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY src/WebAPI/bin/Release/net8.0 .
EXPOSE 8080
ENTRYPOINT [ "dotnet", "WebAPI.dll" ]
