FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY *.sln ./
COPY DevWeek.Worker/DevWeek.Worker.csproj DevWeek.Worker/
COPY DevWeek.WebApp/DevWeek.WebApp.csproj DevWeek.WebApp/
COPY DevWeek.Contracts/DevWeek.Contracts.csproj DevWeek.Contracts/
COPY DevWeek.Services/DevWeek.Services.csproj DevWeek.Services/
COPY DevWeek.Services.Tests/DevWeek.Services.Tests.csproj DevWeek.Services.Tests/
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
