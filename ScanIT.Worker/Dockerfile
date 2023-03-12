FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ScanIT.Worker/ScanIT.Worker.csproj", "ScanIT.Worker/"]
RUN dotnet restore "ScanIT.Worker/ScanIT.Worker.csproj"
COPY . .
WORKDIR "/src/ScanIT.Worker"
RUN dotnet build "ScanIT.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ScanIT.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ScanIT.Worker.dll"]
