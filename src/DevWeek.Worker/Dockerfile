FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
RUN apt-get update && apt-get install curl ffmpeg python3.9-minimal ca-certificates -y && cp /usr/bin/python3.9 /usr/bin/python3 && chmod a+rx /usr/bin/python3
RUN curl -L https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp -o /usr/local/bin/yt-dlp && chmod a+rx /usr/local/bin/yt-dlp


FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN apt-get update && apt-get install curl ffmpeg python3.9-minimal ca-certificates -y && cp /usr/bin/python3.9 /usr/bin/python3 && chmod a+rx /usr/bin/python3
RUN curl -L https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp -o /usr/local/bin/yt-dlp && chmod a+rx /usr/local/bin/yt-dlp
WORKDIR /src
COPY *.sln ./
COPY DevWeek.Worker/DevWeek.Worker.csproj DevWeek.Worker/
COPY DevWeek.WebApp/DevWeek.WebApp.csproj DevWeek.WebApp/
COPY DevWeek.Contracts/DevWeek.Contracts.csproj DevWeek.Contracts/
COPY DevWeek.Services/DevWeek.Services.csproj DevWeek.Services/
COPY DevWeek.Services.Tests/DevWeek.Services.Tests.csproj DevWeek.Services.Tests/
RUN dotnet restore ./src-ci.sln
COPY . .
WORKDIR /src/DevWeek.Worker
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
VOLUME /shared
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DevWeek.Worker.dll"]
