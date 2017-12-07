FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app
RUN apt-get update && apt-get install python2.7-minimal -y && cp /usr/bin/python2.7 /usr/bin/python
RUN curl -L https://yt-dl.org/downloads/latest/youtube-dl -o /usr/local/bin/youtube-dl && chmod a+rx /usr/local/bin/youtube-dl

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
VOLUME /shared
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DevWeekWorker.dll"]
