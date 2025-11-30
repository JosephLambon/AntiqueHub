# Antique Hub

## 🧰 Get Started

### Prerequisites
- [Docker](https://www.docker.com/) installed and running.

Docker images are used to **emulate** the following dependencies locally.

| Image        | Mocked resource |
|--------------|-----------------|
| [Azurite](https://hub.docker.com/r/microsoft/azure-storage-azurite) | Azure Blob Storage |
| [Postgres](https://hub.docker.com/_/postgres) | Azure Database for PostgreSQL |
### Running locally
From your terminal, **run** ```docker compose up```.

You will see the prerequisite images being pulled, followed by database initialisation.  

Expect a successful startup to finish with the following. At this point, you can call the API using the **Postman Collection** provided.  


```
antique-api-1             | info: Microsoft.Hosting.Lifetime[14]
antique-api-1             |       Now listening on: http://[::]:8080
antique-api-1             | info: Microsoft.Hosting.Lifetime[0]
antique-api-1             |       Application started. Press Ctrl+C to shut down.
antique-api-1             | info: Microsoft.Hosting.Lifetime[0]
antique-api-1             |       Hosting environment: Development
antique-api-1             | info: Microsoft.Hosting.Lifetime[0]
antique-api-1             |       Content root path: /app
```

---
## Sources
1. [PostgreSQL](https://hub.docker.com/_/postgres)
2. [Azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-install-azurite?tabs=docker-hub%2Cblob-storage)
