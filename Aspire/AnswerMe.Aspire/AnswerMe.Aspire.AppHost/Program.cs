using AnswerMe.Aspire.AppHost;


var builder = DistributedApplication.CreateBuilder(args);


var answerMeDb=builder.AddPostgres("answerme", password: builder.CreateStablePassword("AnswerMeDB-password"))
    .WithDataVolume()
    .WithPgAdmin()
    .AddDatabase("AnswerMeDB");

var identityServerDb=builder.AddPostgres("identityserver", password: builder.CreateStablePassword("AnswerMeDB-password"))
    .WithDataVolume()
    .WithPgAdmin()
    .AddDatabase("IdentityServerDb");

var redisCache = builder.AddRedis("RedisCache")
    .WithRedisCommander()
    .WithDataVolume();

var table = builder.AddAzureStorage("azureobjectindextable")
    .RunAsEmulator(emulator =>
    {
        emulator.WithTablePort(9000);
        emulator.WithDataVolume();
    })
    .AddTables("ObjectIndexTable");

var blobs = builder.AddAzureStorage("azureobjectstorage")
    .RunAsEmulator(emulator =>
    {
        emulator.WithBlobPort(10000);
        emulator.WithDataVolume();
    })
    .AddBlobs("ObjectStorage");



var identityServerApi= builder.AddProject<Projects.IdentityServer_Api>("IdentityServerApi")
    .WithReference(identityServerDb);

var objectStorageApi= builder.AddProject<Projects.ObjectStorage_Api>("ObjectStorageApi")
    .WithReference(redisCache)
    .WithReference(table)
    .WithReference(blobs)
    .WithReference(identityServerApi);

var answerMeApi = builder.AddProject<Projects.AnswerMe_Api>("AnswerMeApi")
    .WithReference(answerMeDb)
    .WithReference(redisCache)
    .WithReference(blobs)
    .WithReference(objectStorageApi)
    .WithReference(identityServerApi);

var client = builder.AddProject<Projects.AnswerMe_Client>("AnswerMeClient")
    .WithReference(answerMeApi)
    .WithReference(identityServerApi)
    .WithReference(objectStorageApi);




builder.Build().Run();