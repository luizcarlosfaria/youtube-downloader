FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim  AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk-stretch AS build
WORKDIR /src
COPY *.sln ./
COPY DevWeek.WebApp/DevWeek.WebApp.csproj DevWeekWebApp/
RUN dotnet restore ./src-ci.sln
COPY . .
WORKDIR /src/DevWeek.WebApp
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DevWeek.WebApp.dll"]
