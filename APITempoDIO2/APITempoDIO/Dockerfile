FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY ["APITempoDIO/APITempoDIO.csproj", "APITempoDIO/"]
RUN dotnet restore "APITempoDIO/APITempoDIO.csproj"
COPY . .
WORKDIR "/src/APITempoDIO"
RUN dotnet build "APITempoDIO.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "APITempoDIO.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "APITempoDIO.dll"]