# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /project

COPY *.sln .
COPY src/*.csproj ./src/
RUN dotnet restore

COPY src/. ./src/
WORKDIR /project/src
RUN dotnet publish -c release -o /published --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:3.1
WORKDIR /app
EXPOSE 80
COPY --from=build /published ./
ENTRYPOINT ["dotnet", "TesteECS.dll"]