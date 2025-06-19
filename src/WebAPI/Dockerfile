FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine
RUN apk --no-cache add curl
WORKDIR /app
COPY bin/Release/net9.0 .
EXPOSE 8080
ENTRYPOINT [ "dotnet", "WebAPI.dll" ]
