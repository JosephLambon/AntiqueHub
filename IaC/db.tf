# Define pg servers

# Deploy the PostgreSQL Flexible Server ONLY if the environment is 'prod'
resource "azurerm_postgresql_flexible_server" "postgres" {
  count               = var.environment == "prod" ? 1 : 0
  name                = "psql-${local.prefix}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  version             = "17"

  administrator_login    = "psqladmin"
  administrator_password = random_string.db_password[count.index].result # Use a generated secure password
  
  zone                   = "1"
  storage_mb             = var.db_storage_mb
  sku_name               = var.db_sku_name
  
  backup_retention_days  = 7
  geo_redundant_backup_enabled = false
  
  public_network_access_enabled = true
}

# Output the DB connection string for the PROD environment for reference
output "prod_db_connection_string" {
  value = count(azurerm_postgresql_flexible_server.postgres) > 0 ? 
    "Host=${azurerm_postgresql_flexible_server.postgres[0].fqdn};Username=psqladmin@${azurerm_postgresql_flexible_server.postgres[0].name};Password=<SECRET>;Database=postgres" :
    "Non-Production database is shared."
  sensitive = true
}