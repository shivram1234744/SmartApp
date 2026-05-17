# ASP.NET MVC 8 Dockerfile

# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file
COPY *.csproj ./

# Restore dependencies
RUN dotnet restore

# Copy all files
COPY . .

# Build project
RUN dotnet build -c Release -o /app/build

# Publish project
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Run application
ENTRYPOINT ["dotnet", "TrustPlus.dll"]