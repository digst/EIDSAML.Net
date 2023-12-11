To use this session provider for Sql Server, configure the eidsaml.net sessionType to 
eid.saml20.ext.sessionstore.sqlserver.SqlServerSessionStoreProvider, eid.saml20.ext.sessionstore.sqlserver

The tables must be created in the database beforehand. a sql script 'EidSaml_SessionStore_CreateTables.sql' has been added to your project,
you can use to create the neccessary sql tables.

Since session state is stored in Sql tables, a maintenance job is required to ensure sessions are clean up. The provider has an internal job that will clear expired records. 

This provider supports the following configurations:
Connection string "eidsaml:SqlServerSessionStoreProvider" (required)
App setting "eidsaml:SqlServerSessionStoreProvider:Schema" (optional, defaults to "dbo")
App setting "eidsaml:SqlServerSessionStoreProvider:CleanupIntervalSeconds" (optional, defaults to 30)
App setting "eidsaml:SqlServerSessionStoreProvider:DisableCleanup" (optional, default to "false")