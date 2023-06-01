#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
RUN apt-get update && apt-get install -y ffmpeg

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AudioProcessConsumer/AudioProcessConsumer.csproj", "AudioProcessConsumer/"]
RUN dotnet restore "AudioProcessConsumer/AudioProcessConsumer.csproj"
COPY . .
WORKDIR "/src/AudioProcessConsumer"
RUN dotnet build "AudioProcessConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AudioProcessConsumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AudioProcessConsumer.dll"]