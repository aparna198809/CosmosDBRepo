# CosmosDBRepo

A learning and demo repository with small, focused examples related to **Azure Cosmos DB** and adjacent Azure services. Each folder is intended to be an independent “mini-demo” you can explore, run, and adapt.

> **Goal:** provide practical, copy-pastable starting points for common Cosmos DB scenarios (partitioning, bulk operations, pagination, CDC patterns, infra provisioning, etc.).

---

## What’s in this repo

High-level modules currently present:

- **ADFCDCFeatureCosmosDB/** — Experiments/demos related to change data capture patterns using Azure Data Factory + Cosmos DB.
- **ArchiveCosmosToBlob/** — Patterns for archiving/exporting data from Cosmos DB into Azure Blob Storage.
- **Bulk Import/** — Bulk ingestion examples and notes.
- **Cognitive Services OCR/** — Sample integration of OCR (Azure Cognitive Services) with data storage patterns (e.g., writing results to Cosmos DB).
- **Computed Properties/** — Examples demonstrating computed properties concepts/patterns.
- **Hierarchial_Partition_CosmosDB_v1/** — Partitioning experiments and notes (hierarchical partitioning-related learning).
- **PreferredRegions/** — Preferred regions / multi-region client configuration experiments.
- **TableApiCrudOperations/** — CRUD examples using Cosmos DB Table API.
- **TerraformDeployCosmosDB/** — Infrastructure-as-code demos for provisioning Cosmos DB using Terraform.
- **pagination/** — Pagination patterns (continuation tokens, query paging, etc.).
- **transactional batch/** — Transactional batch examples.

There is also:
- **GettingStartedWithNotebooks.ipynb** — A large notebook for hands-on exploration.

---

## Getting started

Because this repository contains multiple independent demos, the first step is to pick a module folder and follow its local instructions (or inspect its code/notebooks).

### Prerequisites (common)
Depending on the module you choose, you may need:

- An **Azure subscription**
- A **Cosmos DB account** (SQL API / Table API depending on the demo)
- Azure CLI (optional, but useful)
- Terraform (for `TerraformDeployCosmosDB/`)
- A language runtime (varies by module; see the module folder)

---

## Suggested repo conventions (how to use this repo)

If you add new demos, consider using this lightweight structure:

- Each demo folder should include:
  - a short `README.md` explaining what it demonstrates, prerequisites, and how to run it
  - a `sample.env` (never commit secrets)
  - minimal steps to reproduce results

This top-level README stays focused on *discoverability* across demos.

---

## Safety and secrets

- Do **not** commit Cosmos DB keys, connection strings, or Azure credentials.
- Prefer environment variables and local configuration files excluded via `.gitignore`.

---

## License

This repository is licensed under the MIT License. See `LICENSE`.

---

## Disclaimer

This repository is for **learning and demonstration** purposes. It is not production-hardened and may omit concerns like robust error handling, security hardening, and cost controls.
