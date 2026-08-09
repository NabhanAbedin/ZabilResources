FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Zabil.Api/Zabil.Api.csproj Zabil.Api/
RUN dotnet restore Zabil.Api/Zabil.Api.csproj

COPY Zabil.Api/ Zabil.Api/
RUN dotnet publish Zabil.Api/Zabil.Api.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
USER app
ENTRYPOINT ["dotnet", "Zabil.Api.dll"]