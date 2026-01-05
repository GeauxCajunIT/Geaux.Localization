
IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server container with custom settings
IResourceBuilder<ParameterResource> msqlPassword = builder.AddParameter("Mssql-Password", secret: true);

IResourceBuilder<SqlServerServerResource> sql = builder.AddSqlServer("sqlserver", password: msqlPassword)
    .WithImage("mssql/server") // custom image
    .WithImageTag("2025-latest")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("MSSQL_PID", "Developer")
    .WithVolume("sql_data", "/var/opt/mssql") // persistent volume
    .WithHostPort(2617);// optional: expose port

IResourceBuilder<ProjectResource> apiService = builder.AddProject<Projects.Geaux_Localization_AspireSample_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Geaux_Localization_AspireSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(sql)
    .WaitFor(sql);


builder.Build().Run();
