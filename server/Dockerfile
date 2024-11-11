FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
RUN apk --no-cache add curl
WORKDIR /app
COPY src/WebAPI/bin/Release/net8.0 .
EXPOSE 8080
ENTRYPOINT [ "dotnet", "WebAPI.dll" ]
