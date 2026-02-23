var builder = DistributedApplication.CreateBuilder(args);

var postgredb = builder.AddPostgres("postgres")
    .WithDataVolume("postgres-wooai")
    .WithPgWeb(pgWeb => pgWeb.WithHostPort(5050))
    .AddDatabase("dev-wooai");

var rabbitmq = builder.AddRabbitMQ("eventbus")
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

var migration = builder.AddProject<Projects.Dev_WooAI_MigrationsDbWorkerService>("wooai-migration")
    .WithReference(postgredb)
    .WaitFor(postgredb);

var httpApi=builder.AddProject<Projects.Dev_WooAI_HttpApi>("dev-wooai-httpapi")
    .WithUrl("swagger")
    .WaitFor(postgredb)
    .WaitFor(rabbitmq)
    .WithReference(postgredb)
    .WithReference(rabbitmq)
    .WaitForCompletion(migration);

builder.Build().Run();
