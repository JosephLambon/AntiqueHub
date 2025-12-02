# We use Azure Storage (azurerm) to securely store the Terraform state file.
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=4.1.0"
    }
  }
  
  backend "azurerm" {
        # REQUIRES INPUTS FROM CONFIG
      
        # Commenting out defaults
        #     resource_group_name  = "rg-tfstate"                     
        #     storage_account_name = "tfstatejoelambon"               # Global
        #     container_name       = "tfstate-container"              # Global
        #     key                  = "antique_api/terraform.tfstate"  # Configure per app/env
  }
}

provider "azurerm" {
  features {}
}