FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY *.sln ./
COPY DevWeekWorker/DevWeekWorker.csproj DevWeekWorker/
RUN dotnet restore
COPY . .
WORKDIR /src/DevWeekWorker
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DevWeekWorker.dll"]
