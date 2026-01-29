var builder = DistributedApplication.CreateBuilder(args);

var postgredb = builder.AddPostgres("postgres")
    .WithDataVolume("postgres-wooai")
    .WithPgWeb(pgWeb => pgWeb.WithHostPort(5050))
    .AddDatabase("dev-wooai");

var migration = builder.AddProject<Projects.Dev_WooAI_MigrationsDbWorkerService>("wooai-migration")
    .WithReference(postgredb)
    .WaitFor(postgredb);

var httpApi=builder.AddProject<Projects.Dev_WooAI_HttpApi>("dev-wooai-httpapi")
    .WithUrl("swagger")
    .WaitFor(postgredb)
    .WithReference(postgredb)
    .WaitForCompletion(migration);

builder.Build().Run();
