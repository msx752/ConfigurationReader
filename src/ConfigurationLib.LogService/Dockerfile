#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ConfigurationLib.LogService/ConfigurationLib.LogService.csproj", "src/ConfigurationLib.LogService/"]
COPY ["src/libs/ConfigurationLib/ConfigurationLib.csproj", "src/libs/ConfigurationLib/"]
COPY ["src/Shared.Kernel/Shared.Kernel.csproj", "src/Shared.Kernel/"]
RUN dotnet restore "./src/ConfigurationLib.LogService/ConfigurationLib.LogService.csproj"
COPY . .
WORKDIR "/src/src/ConfigurationLib.LogService"
RUN dotnet build "./ConfigurationLib.LogService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ConfigurationLib.LogService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConfigurationLib.LogService.dll"]