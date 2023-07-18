FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["StudentQueries/StudentQueries.csproj", "StudentQueries/"]
RUN dotnet restore "StudentQueries/StudentQueries.csproj"
COPY . .
WORKDIR "/src/StudentQueries"
RUN dotnet build "StudentQueries.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StudentQueries.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudentQueries.dll"]
