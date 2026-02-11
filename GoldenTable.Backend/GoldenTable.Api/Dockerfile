FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GoldenTable.Api/GoldenTable.Api.csproj", "GoldenTable.Api/"]
COPY ["GoldenTable.Common.Domain/GoldenTable.Common.Domain.csproj", "GoldenTable.Common.Domain/"]
COPY ["GoldenTable.Common.Presentation/GoldenTable.Common.Presentation.csproj", "GoldenTable.Common.Presentation/"]
COPY ["GoldenTable.Common.Infrastructure/GoldenTable.Common.Infrastructure.csproj", "GoldenTable.Common.Infrastructure/"]
COPY ["GoldenTable.Common.Application/GoldenTable.Common.Application.csproj", "GoldenTable.Common.Application/"]
RUN dotnet restore "GoldenTable.Api/GoldenTable.Api.csproj"
COPY . .
WORKDIR "/src/GoldenTable.Api"
RUN dotnet build "./GoldenTable.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GoldenTable.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoldenTable.Api.dll"]
