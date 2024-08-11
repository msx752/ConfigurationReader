#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ConfigurationLib.Dashboard.Api/ConfigurationLib.Dashboard.Api.csproj", "src/ConfigurationLib.Dashboard.Api/"]
RUN dotnet restore "./src/ConfigurationLib.Dashboard.Api/ConfigurationLib.Dashboard.Api.csproj"
COPY . .
WORKDIR "/src/src/ConfigurationLib.Dashboard.Api"
RUN dotnet build "./ConfigurationLib.Dashboard.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConfigurationLib.Dashboard.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConfigurationLib.Dashboard.Api.dll"]