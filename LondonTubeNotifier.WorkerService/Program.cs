using LondonTubeNotifier.WorkerService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<TflLineStatusWorker>();

var host = builder.Build();
host.Run();
