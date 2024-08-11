#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ConfigurationLib.Dashboard/ConfigurationLib.Dashboard.csproj", "src/ConfigurationLib.Dashboard/"]
COPY ["src/libs/ConfigurationLib/ConfigurationLib.csproj", "src/libs/ConfigurationLib/"]
RUN dotnet restore "./src/ConfigurationLib.Dashboard/ConfigurationLib.Dashboard.csproj"
COPY . .
WORKDIR "/src/src/ConfigurationLib.Dashboard"
RUN dotnet build "./ConfigurationLib.Dashboard.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConfigurationLib.Dashboard.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConfigurationLib.Dashboard.dll"]