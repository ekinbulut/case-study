FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5065

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/NotificationService/NotificationService.API/NotificationService.API.csproj", "NotificationService.API/"]
COPY ["src/NotificationService/NotificationService.Domain/NotificationService.Domain.csproj", "NotificationService.Domain/"]
COPY ["src/NotificationService/NotificationService.Infrastructure/NotificationService.Infrastructure.csproj", "NotificationService.Infrastructure/"]
COPY ["src/NotificationService/NotificationService.Application/NotificationService.Application.csproj", "NotificationService.Application/"]
COPY ["src/Common/Common/Common.csproj", "/Common/Common/"]

RUN dotnet restore "NotificationService.API/NotificationService.API.csproj"
# Copy the entire project files
COPY src/NotificationService/ NotificationService/
COPY src/Common/ Common/

WORKDIR "/src/NotificationService/NotificationService.API"
RUN dotnet build "NotificationService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "NotificationService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NotificationService.API.dll"]
