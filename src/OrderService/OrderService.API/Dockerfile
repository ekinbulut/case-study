FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5066

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/OrderService/OrderService.API/OrderService.API.csproj", "OrderService.API/"]
COPY ["src/OrderService/OrderService.Domain/OrderService.Domain.csproj", "OrderService.Domain/"]
COPY ["src/OrderService/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "OrderService.Infrastructure/"]
COPY ["src/OrderService/OrderService.Application/OrderService.Application.csproj", "OrderService.Application/"]
COPY ["src/Common/Common/Common.csproj", "/Common/Common/"]

RUN dotnet restore "OrderService.API/OrderService.API.csproj"
# Copy the entire project files
COPY src/OrderService/ OrderService/
COPY src/Common/ Common/

WORKDIR "/src/OrderService/OrderService.API"
RUN dotnet build "OrderService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.API.dll"]
