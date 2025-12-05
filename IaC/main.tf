# Deploys specified Azure resources

# Define a consistent naming prefix
locals {
  prefix = "${var.app_name}-${var.environment}"
  # Dev and Test can share a non-prod plan to save costs.
  app_service_plan_name = var.environment == "prod" ? "${local.prefix}-asp" : "${var.app_name}-nonprod-asp"
}

# Resource Group
resource "azurerm_resource_group" "rg" {
  name     = "rg-${local.prefix}"
  location = var.location
}

# Application Insights
resource "azurerm_application_insights" "app_insights" {
  name                = "appins-${local.prefix}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
  workspace_id        = null # Use default settings for simplicity
}

# App Service Plan (Shared for non-prod)
resource "azurerm_service_plan" "asp" {
  name                = local.app_service_plan_name
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = var.app_service_sku_tier
}

# App Service (The API Backend)
resource "azurerm_linux_web_app" "app_service" {
  name                = "app-${local.prefix}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id     = azurerm_service_plan.asp.id
  enabled             = true
  
  identity {
    type             = "SystemAssigned"
  }
  
  site_config {
    always_on        = false
    scm_minimum_tls_version = "1.2"
    application_stack {
      dotnet_version = "8.0" 
    }
  }

  # Application Settings (Environment Variables)
  app_settings = {
    "ASPNETCORE_ENVIRONMENT"                = var.environment
    "APPLICATIONINSIGHTS_CONNECTION_STRING" = azurerm_application_insights.app_insights.connection_string
  }
}

# Storage Account (General purpose, e.g., for file uploads/metadata)
resource "azurerm_storage_account" "storage" {
  name                     = "storage${replace(local.prefix, "-", "")}" # Must be globally unique and lowercase
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location 
  account_tier             = "Standard"
  account_replication_type = "LRS"
  account_kind             = "StorageV2"
}

# Blob Container with Anonymous Read Access
resource "azurerm_storage_container" "image_container" {
  name                  = "antique-image-container"
  storage_account_name  = azurerm_storage_account.storage.name
  container_access_type = "blob"
}