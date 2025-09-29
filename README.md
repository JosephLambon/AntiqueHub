# Document Middleware

## 🧰 Get Started
1. **Install** Docker
2. **Pull** Docker Images
- Postgres - database mocking for antique items
- Azurite - emulating Blob Storage for document uploads
```
docker pull postgres
docker run --name postgres-db -e POSTGRES_PASSWORD=testpassword -e POSTGRES_USER=testuser -e POSTGRES_DB=mydatabase -p 5432:5432 -v postgres-data:/var/lib/postgresql/data -d postgres
docker ps # confirm it's running
```
```
docker pull mcr.microsoft.com/azure-storage/azurite
docker run --name azurite -d -p 10000:10000 -p 10001:10001 -p 10002:10002 \
    mcr.microsoft.com/azure-storage/azurite
```


3. **Configure** appsettings.json with your PostgreSQL connection string

```
"ConnectionStrings": {
	"default": "Server=localhost,5432;Database=mydatabase;User Id=testuser;Password=testpassword;"
}
```

4. **Run** the application

---
#### Sources
1. PostgreSQL - https://www.datacamp.com/tutorial/postgresql-docker?dc_referrer=https%3A%2F%2Fwww.google.com%2F
2. Azurite -  https://learn.microsoft.com/en-us/azure/storage/common/storage-install-azurite?tabs=docker-hub%2Cblob-storage