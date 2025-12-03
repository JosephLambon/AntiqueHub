# Define pg servers

# Deploy the PostgreSQL Flexible Server ONLY if the environment is 'prod'
resource "azurerm_postgresql_flexible_server" "postgres" {
  count               = var.environment == "prod" ? 1 : 0
  name                = "psql-${local.prefix}-${var.environment}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  version             = "17"

  administrator_login    = "psqladmin"
  administrator_password = var.db_password
  
  zone                   = "1"
  storage_mb             = var.db_storage_mb
  sku_name               = var.db_sku_name
  
  backup_retention_days  = 7
  geo_redundant_backup_enabled = false
  
  public_network_access_enabled = true
}

# Output the DB connection string for the PROD environment for reference in next steps
# output "prod_db_connection_string" {
#   value = var.create_database && count(azurerm_postgresql_flexible_server.postgres) > 0 ? 
#       "Host=${azurerm_postgresql_flexible_server.postgres[0].fqdn};Username=psqladmin@${azurerm_postgresql_flexible_server.postgres[0].name};Password=${var.db_password};Database=postgres" : 
#       "Database deployment skipped or connection string not available."
#   sensitive = true
# }