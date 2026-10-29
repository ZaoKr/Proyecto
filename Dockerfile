FROM node:22-bookworm-slim AS frontend

WORKDIR /src/frontend

COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci --no-audit --no-fund

COPY frontend/ ./
RUN npm run build


FROM mcr.microsoft.com/dotnet/sdk:10.0 AS backend

WORKDIR /src

COPY backend/ ./backend/
RUN dotnet publish backend/src/Cafeteria.Api/Cafeteria.Api.csproj -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=backend /app/publish ./
COPY --from=frontend /src/frontend/dist/Frontend/browser/ ./wwwroot/

ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "Cafeteria.Api.dll"]
