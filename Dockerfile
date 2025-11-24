FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG TARGETARCH
WORKDIR /source

COPY --link AntiqueHub.Api/AntiqueHub.Api.csproj AntiqueHub.Api/
COPY --link AntiqueHub.Core/AntiqueHub.Core.csproj AntiqueHub.Core/
RUN dotnet restore AntiqueHub.Api/AntiqueHub.Api.csproj -a $TARGETARCH

COPY --link AntiqueHub.Core AntiqueHub.Core
COPY --link AntiqueHub.Api AntiqueHub.Api

RUN dotnet publish AntiqueHub.Api/AntiqueHub.Api.csproj --no-restore -a $TARGETARCH -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --link --from=build /app .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AntiqueHub.Api.dll"]