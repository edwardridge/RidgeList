#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-focal AS build

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
RUN dotnet restore "RidgeList.FrontEnd/RidgeList.FrontEnd.csproj" --configfile=./RidgeList.FrontEnd/nuget.config
COPY . .
WORKDIR "/src/RidgeList.FrontEnd"
RUN dotnet build "RidgeList.FrontEnd.csproj" -c Release -o /app/build

RUN dotnet test "../RidgeList.Domain.Tests/RidgeList.Domain.Tests.csproj"

FROM build AS publish
RUN dotnet publish "RidgeList.FrontEnd.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RidgeList.FrontEnd.dll"]