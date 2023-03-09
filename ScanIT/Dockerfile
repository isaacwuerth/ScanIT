FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ScanIT/ScanIT.csproj", "ScanIT/"]
RUN dotnet restore "ScanIT/ScanIT.csproj"
COPY . .
WORKDIR "/src/ScanIT"
RUN dotnet build "ScanIT.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ScanIT.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScanIT.dll"]
