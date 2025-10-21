# Antique Hub

## 🧰 Get Started

### Prerequisites
- [Docker](https://www.docker.com/) installed and running.
- [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer#Download-4) installed and running.

### Running locally

This API uses **Azurite** to emulate Azure Blob Storage locally.

1. In Azure Storage Explorer, create a:
   1. Storage Account named _'devstoreaccount1'_
   2. Container within it named _'antique-image-container'_.
2. **Pull** Docker Images
   1. Postgres - database mocking for antique items 
   2. Azurite - emulating Blob Storage for document uploads
    ```
    docker pull postgres
    docker run --name postgres-db -e POSTGRES_PASSWORD=testpassword -e POSTGRES_USER=testuser -e POSTGRES_DB=mydatabase -p 5432:5432 -v postgres-data:/var/lib/postgresql/data -d postgres
    ```
    ```
    docker pull mcr.microsoft.com/azure-storage/azurite
    docker run --name azurite -d -p 10000:10000 -p 10001:10001 -p 10002:10002 \
        mcr.microsoft.com/azure-storage/azurite
    ```
    ```
    docker ps # confirm containers are running
   
    # Example output
    CONTAINER ID   IMAGE                                     COMMAND                  CREATED       STATUS              PORTS                                                                     NAMES
    b5cc9a5c3b4d   mcr.microsoft.com/azure-storage/azurite   "docker-entrypoint.s…"   3 weeks ago   Up About a minute   0.0.0.0:10000-10002->10000-10002/tcp, [::]:10000-10002->10000-10002/tcp   azurite
    8cf6f9c832ef   postgres                                  "docker-entrypoint.s…"   3 weeks ago   Up About a minute   0.0.0.0:5432->5432/tcp, [::]:5432->5432/tcp                               postgres-db
    ```
   

4. **Run** the application

---
## Sources
1. [PostgreSQL](https://hub.docker.com/_/postgres)
2. [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-install-azurite?tabs=docker-hub%2Cblob-storage)
