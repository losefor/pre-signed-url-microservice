# Use official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /app

# Copy project files
# COPY *.sln ./
# COPY API/*.csproj API/
# COPY Domain/*.csproj Domain/
# COPY Helpers/*.csproj Helpers/

COPY . .

# Restore dependencies
RUN dotnet restore

# Copy the rest of the app files

# Build and publish the app
RUN dotnet publish "API/API.csproj" -c Release -o /publish --no-restore

# Use runtime image for production
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Set working directory
WORKDIR /app

# Copy published app from the build stage
COPY --from=build /publish .

# Expose application port
EXPOSE 5001

# Start the application
ENTRYPOINT ["dotnet", "API.dll"]
