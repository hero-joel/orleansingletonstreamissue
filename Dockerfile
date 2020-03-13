FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
#COPY *.sln ./
COPY Host/Host.csproj Host/
COPY Directory.Build.props .
COPY nuget.config .
WORKDIR /src
COPY . .
WORKDIR /src/Host
#RUN dotnet restore /p:RestoreUseSkipNonexistentTargets="false"
RUN dotnet publish -c Release -o /app

#FROM build AS publish
#RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Host.dll"]
