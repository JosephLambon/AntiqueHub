# Define schema per environment
variable "environment" {
  description = "The target deployment environment (e.g., dev, test, prod)."
  type        = string
}

variable "location" {
  description = "The Azure region to deploy resources into."
  type        = string
  default     = "UK South"
}

variable "app_name" {
  description = "A unique name for the application, used in resource naming."
  type        = string
}

variable "app_service_sku_tier" {
  description = "The SKU tier for the App Service Plan (e.g., Basic, Standard, Premium)."
  type        = string
  default     = "F1"
}

variable "db_sku_name" {
  description = "The SKU for the PostgreSQL Flexible Server (e.g., GP_Standard_D2ds_v4)."
  type        = string
}

variable "db_storage_mb" {
  description = "Storage size in megabytes for the PostgreSQL server."
  type        = number
}

variable "db_admin_login" {
  description = "The administrator username for the PostgreSQL server."
  type        = string
  default     = "psqladmin"
}

variable "db_password" {
  description = "The admin password for the PostgreSQL server."
  type        = string
  sensitive   = true
}

variable "postgres_connection_string" {
  description = "The connection string for the PostgreSQL database."
  type        = string
  sensitive   = true
}