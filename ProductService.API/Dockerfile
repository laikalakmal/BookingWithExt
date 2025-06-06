FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ["*.sln", "./"]

# Copy project files for restore
COPY ["Core.Application/*.csproj", "Core.Application/"]
COPY ["Core.Domain/*.csproj", "Core.Domain/"]
COPY ["Infrastructure.Adapters/*.csproj", "Infrastructure.Adapters/"]
COPY ["Infrastructure.Persistence/*.csproj", "Infrastructure.Persistence/"]
COPY ["ProductService.API/ProductServiceAPI.csproj", "ProductService.API/"]
#COPY ["Core.ApplicationTests/*.csproj", "Core.ApplicationTests/"]

# Restore dependencies
RUN dotnet restore "ProductService.API/ProductServiceAPI.csproj"

# Copy the rest of the code
COPY . .

# Build the application (excluding tests)
WORKDIR /src/ProductService.API
RUN dotnet build -c Release --no-restore

# Publish the application
FROM build AS publish
WORKDIR /src/ProductService.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Set environment variables - HTTP only for development
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

# Add healthcheck
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:80/health || exit 1

COPY --from=publish /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "ProductServiceAPI.dll"]