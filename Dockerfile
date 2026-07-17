FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY PromptlyNote.Api/PromptlyNote.Api.csproj PromptlyNote.Api/
COPY PromptlyNote.Core/PromptlyNote.Core.csproj PromptlyNote.Core/
COPY PromptlyNote.Data/PromptlyNote.Data.csproj PromptlyNote.Data/
COPY PromptlyNote.Services/PromptlyNote.Services.csproj PromptlyNote.Services/
RUN dotnet restore PromptlyNote.Api/PromptlyNote.Api.csproj

COPY . .
RUN dotnet publish PromptlyNote.Api/PromptlyNote.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=5001

EXPOSE 5001

COPY --from=build /app/publish .

USER $APP_UID

ENTRYPOINT ["dotnet", "PromptlyNote.Api.dll"]
