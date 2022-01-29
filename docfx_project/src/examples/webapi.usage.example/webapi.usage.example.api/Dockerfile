#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["webapi.usage.example.api/webapi.usage.example.api.csproj", "webapi.usage.example.api/"]
RUN dotnet restore "webapi.usage.example.api/webapi.usage.example.api.csproj"
COPY . .
WORKDIR "/src/webapi.usage.example.api"
RUN dotnet build "webapi.usage.example.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "webapi.usage.example.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "webapi.usage.example.api.dll"]