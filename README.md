# Storage and Post Service

## Overview
This project consists of two microservices:

1. **Storage Service** - Handles file uploads, generates signed URLs, and validates file existence.
2. **Post Service** - Allows users to create posts that reference files uploaded via the Storage Service. It verifies the file's existence before accepting the post.

Both services communicate using **Docker Compose** and interact over HTTP.

## Features
- **Storage Service**:
  - Generate signed URLs for secure file uploads.
  - Upload files and store metadata.
  - Validate file existence.
- **Post Service**:
  - Create posts with a reference to an uploaded file.
  - Validate the file's existence before saving the post.
  - Retrieve posts by ID.
- **Docker Compose Integration** for seamless inter-service communication.

## Tech Stack
- **.NET 8** (ASP.NET Core Web API)
- **C#**
- **Docker & Docker Compose**
- **HTTP Client for Service Communication**
- **Concurrent Dictionary for In-Memory Storage**

## Installation & Setup

### Prerequisites
Ensure you have the following installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [Docker & Docker Compose](https://docs.docker.com/compose/install/)
- [Git](https://git-scm.com/downloads)

### Clone the Repository
```sh
git clone https://github.com/losefor/pre-signed-url-microservice
cd pre-signed-url-microservice
```

### Running with Docker Compose
1. **Build & Start the Services**:
```sh
docker-compose up --build
```
2. The services should now be running:
   - **Storage Service** → `http://localhost:5001/api/storage`
   - **Post Service** → `http://localhost:5002/api/posts`

### Running Locally (Without Docker)
1. Open the **Storage Service** in a terminal:
```sh
cd StorageService
dotnet run
```
2. Open another terminal for **Post Service**:
```sh
cd PostService
export StorageService__Url="http://localhost:5001/api/storage"
dotnet run
```

## API Endpoints

### **Storage Service** (`http://localhost:5001/api/storage`)
| Method  | Endpoint                  | Description                |
|---------|---------------------------|----------------------------|
| `POST`  | `/signed-url`             | Generate a signed upload URL |
| `POST`  | `/upload`                 | Upload a file |
| `GET`   | `/exists/{fileId}`        | Check if a file exists |

### **Post Service** (`http://localhost:5002/api/posts`)
| Method  | Endpoint                 | Description                 |
|---------|--------------------------|-----------------------------|
| `POST`  | `/`                      | Create a new post (validates file existence) |
| `GET`   | `/{postId}`              | Retrieve a post by ID |

## Environment Variables
| Variable                  | Description                            |
|---------------------------|----------------------------------------|
| `StorageService__Url`     | URL of the Storage Service (used in Post Service) |

To set the variable locally:
```sh
export StorageService__Url="http://localhost:5001/api/storage"
```

For Docker Compose, it is set in `docker-compose.yml`:
```yaml
environment:
  - StorageService__Url=http://storage:5001/api/storage
```
