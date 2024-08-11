#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["tests/ConfigurationLib.Tests/ConfigurationLib.Tests.csproj", "tests/ConfigurationLib.Tests/"]
COPY ["src/libs/ConfigurationLib/ConfigurationLib.csproj", "src/libs/ConfigurationLib/"]
RUN dotnet restore "./tests/ConfigurationLib.Tests/ConfigurationLib.Tests.csproj"
COPY . .
WORKDIR "/src/tests/ConfigurationLib.Tests"
RUN dotnet build "./ConfigurationLib.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConfigurationLib.Tests.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConfigurationLib.Tests.dll"]