FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5067

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/StockService/StockService.API/StockService.API.csproj", "StockService.API/"]
COPY ["src/StockService/StockService.Domain/StockService.Domain.csproj", "StockService.Domain/"]
COPY ["src/StockService/StockService.Infrastructure/StockService.Infrastructure.csproj", "StockService.Infrastructure/"]
COPY ["src/StockService/StockService.Application/StockService.Application.csproj", "StockService.Application/"]
COPY ["src/Common/Common/Common.csproj", "/Common/Common/"]

RUN dotnet restore "StockService.API/StockService.API.csproj"
# Copy the entire project files
COPY src/StockService/ StockService/
COPY src/Common/ Common/

WORKDIR "/src/StockService/StockService.API"
RUN dotnet build "StockService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "StockService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY wait-for-it.sh /wait-for-it.sh

RUN chmod +x /wait-for-it.sh

ENTRYPOINT ["/wait-for-it.sh", "stock-db:5432", "--","/wait-for-it.sh", "rabbitmq:5672", "--", "dotnet", "StockService.API.dll"]
