FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Astor.Background.Management.Service.csproj", ""]
RUN dotnet restore "Astor.Background.Management.Service.csproj"
COPY . .
WORKDIR "/src"
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
    --InternalExchangePrefix=$INTERNALEXCHANGEPREFIX