# Future Learning

Topics to explore after completing the current Docker and modular architecture exercises.

## PostgreSQL extensions

- Understand what PostgreSQL extensions are and how they add database capabilities.
- Learn how to discover, install, enable, upgrade, and remove extensions safely.
- Explore `pg_trgm` for indexed substring and fuzzy URL searches.
- Explore `citext` for case-insensitive text values and compare it with `ILIKE`.
- Explore `pgvector` for vector similarity search and AI-related use cases.
- Explore PostGIS for geospatial data and queries.
- Compare extension-backed features with application-level or dedicated-service alternatives.
- Understand portability, security, performance, backup, and managed-service support trade-offs before adopting an extension.
- Add a practical exercise that enables `pg_trgm`, creates a GIN trigram index on `UrlEntries.OriginalUrl`, and compares query plans before and after indexing.
