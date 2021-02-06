#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base

# https://github.com/GoogleCloudPlatform/google-cloud-dotnet-powerpack/issues/22
RUN apt-get update
RUN apt-get install -y libc-dev

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build

# Install NodeJS 
RUN apt-get update
RUN apt-get -y install gnupg2
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN apt-get install -y nodejs

WORKDIR /src
COPY ["RidgeList.FrontEnd/RidgeList.FrontEnd.csproj", "RidgeList.FrontEnd/"]
COPY ["RidgeList.FrontEnd/nuget.config", "RidgeList.FrontEnd/nuget.config"]
COPY ["RidgeList.Domain/RidgeList.Domain.csproj", "RidgeList.Domain/"]
COPY ["RidgeList.Domain.Tests/RidgeList.Domain.Tests.csproj", "RidgeList.Domain.Tests/"]
COPY ["RidgeList.Postgres/RidgeList.Postgres.csproj", "RidgeList.Postgres/"]
COPY ["RidgeList.Models/RidgeList.Models.csproj", "RidgeList.Models/"]
COPY ["RidgeList.ApplicationServices/RidgeList.ApplicationServices.csproj", "RidgeList.ApplicationServices/"]
RUN dotnet restore "RidgeList.FrontEnd/RidgeList.FrontEnd.csproj" --configfile=./RidgeList.FrontEnd/nuget.config
COPY . .
WORKDIR "/src/RidgeList.FrontEnd"
RUN dotnet publish "RidgeList.FrontEnd.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RidgeList.FrontEnd.dll"]