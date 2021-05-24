FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["Astor.Background.ElasticLogs.Service/Astor.Background.ElasticLogs.Service.csproj", "Astor.Background.ElasticLogs.Service/"]

RUN dotnet restore "Astor.Background.ElasticLogs.Service/Astor.Background.ElasticLogs.Service.csproj"
COPY . .
WORKDIR "/src/Astor.Background.ElasticLogs.Service"
RUN dotnet build "Astor.Background.ElasticLogs.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Astor.Background.ElasticLogs.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT dotnet Astor.Background.ElasticLogs.Service.dll \ 
    --ConnectionStrings:RabbitMq=$RABBIT_CONNECTIONSTRING \
    --Elastic=$ELASTIC_URL \
    --Kibana=$KIBANA_URL \