terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.64.0"
    }
  }

  required_version = ">= 1.1.0"
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = var.rgname
  location = "eastus"
}

resource "azurerm_cosmosdb_account" "act" {
  name                = var.accountname
  resource_group_name = "${azurerm_resource_group.rg.name}"
  location = "eastus"
  consistency_policy {
    consistency_level = "Session"
  }
   geo_location {
    location          = "eastus"
    failover_priority = 0
  }
  offer_type = "Standard"
}

resource "azurerm_cosmosdb_sql_database" "db" {
    name = var.dbname
    resource_group_name = "${azurerm_resource_group.rg.name}"
    account_name = "${azurerm_cosmosdb_account.act.name}"
  
}

resource "azurerm_cosmosdb_sql_container" "collection" {
    name = var.colname
    resource_group_name = "${azurerm_resource_group.rg.name}"
    account_name = "${azurerm_cosmosdb_account.act.name}"
    database_name = "${azurerm_cosmosdb_sql_database.db.name}"
    partition_key_path = "/id"   
  
}



