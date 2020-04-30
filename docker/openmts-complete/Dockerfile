# --- 1: Build frontend ---
FROM node:lts-alpine as frontend-build-env
WORKDIR /app
COPY ../../mts-frontend/package*.json ./
RUN npm install
COPY ../../mts-frontend .
RUN npm run build

# --- 2: Build backend ---
FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS backend-build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ../../mts-backend/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ../../mts-backend/ ./
RUN dotnet publish -c Release -o out

# --- 3: Build runtime ---
FROM mcr.microsoft.com/dotnet/core/aspnet:2.1
WORKDIR /app
COPY --from=backend-build-env /app/out .
COPY --from=frontend-build-env /app/dist ./wwwroot
ENTRYPOINT ["dotnet", "OpenMTS.dll"]