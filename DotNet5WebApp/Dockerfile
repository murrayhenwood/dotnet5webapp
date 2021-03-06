#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["DotNet5WebApp/DotNet5WebApp.csproj", "DotNet5WebApp/"]
RUN dotnet restore "DotNet5WebApp/DotNet5WebApp.csproj"
COPY . .
WORKDIR "/src/DotNet5WebApp"
RUN dotnet build "DotNet5WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DotNet5WebApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DotNet5WebApp.dll"]