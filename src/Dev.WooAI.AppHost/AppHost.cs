using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgredb = builder.AddPostgres("postgres")
    .WithDataVolume("postgres-wooai")
    .WithPgWeb(pgWeb => pgWeb.WithHostPort(5050))
    .AddDatabase("dev-wooai");

var rabbitmq = builder.AddRabbitMQ("eventbus")
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent);

var qdrant = builder.AddQdrant("qdrant")
    .WithLifetime(ContainerLifetime.Persistent);

var migration = builder.AddProject<Projects.Dev_WooAI_MigrationsDbWorkerService>("wooai-migration")
    .WithReference(postgredb)
    .WaitFor(postgredb);

var httpApi=builder.AddProject<Projects.Dev_WooAI_HttpApi>("dev-wooai-httpapi")
    .WithUrl("swagger")
    .WaitFor(postgredb)
    .WaitFor(rabbitmq)
    .WaitFor(qdrant)
    .WithReference(postgredb)
    .WithReference(rabbitmq)
    .WithReference(qdrant)
    .WaitForCompletion(migration);


builder.AddProject<Dev_WooAI_RagWorker>("dev-wooai-ragworker")
    .WithReference(postgredb) // 注入数据库连接
    .WithReference(rabbitmq)   // 注入 RabbitMQ 连接
    .WithReference(qdrant)
    .WaitFor(postgredb)       // 等待数据库启动
    .WaitFor(rabbitmq)        // 等待 MQ 启动
    .WaitFor(qdrant);

builder.Build().Run();
