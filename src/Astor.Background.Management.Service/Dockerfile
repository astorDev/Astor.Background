FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS build
WORKDIR /src
COPY ["Astor.Background.Management.Service/Astor.Background.Management.Service.csproj", "Astor.Background.Management.Service/"]
COPY ["Astor.Background/Astor.Background.csproj", "Astor.Background/"]
COPY ["Astor.Timers/Astor.Timers.csproj", "Astor.Timers/"]

RUN dotnet restore "Astor.Background.Management.Service/Astor.Background.Management.Service.csproj"
COPY . .
WORKDIR "/src/Astor.Background.Management.Service"
RUN dotnet build "Astor.Background.Management.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Astor.Background.Management.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT dotnet Astor.Background.Management.Service.dll \ 
    --ConnectionStrings:Rabbit=$RABBIT_CONNECTIONSTRING  \ 
    --ConnectionStrings:Mongo=$MONGO_CONNECTIONSTRING \
    --Telegram:Token=$TELEGRAM_TOKEN \
    --Telegram:ChatId=$TELEGRAM_CHATID \
    --Mongo:DbName=$MONGO_DBNAME \
    --InternalExchangePrefix=$INTERNALEXCHANGEPREFIX \
    --TimeZoneShift=$TIMEZONESHIFT