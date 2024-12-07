using AnswerMe.Aspire.AppHost;
using Json.More;


var builder = DistributedApplication.CreateBuilder(args);

var username = "postgres";
var password = "Password123";
var answerMeDb=builder.AddPostgres("answerme")
    .WithPgWeb()
    .WithDataVolume()
    .WithPgAdmin()
    .AddDatabase("AnswerMeDB");

var identityServerDb=builder.AddPostgres("identityserver")
    .WithPgWeb()
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
        emulator.WithArgs("azurite", "-l", "/data", "--blobHost", "0.0.0.0", "--queueHost", "0.0.0.0", "--tableHost",
            "0.0.0.0", "--skipApiVersionCheck");
        emulator.WithDataVolume();
    })
    .AddTables("ObjectIndexTable");

var blobs = builder.AddAzureStorage("azureobjectstorage")
    .RunAsEmulator(emulator =>
    {
        emulator.WithBlobPort(10000);
        emulator.WithArgs("azurite", "-l", "/data", "--blobHost", "0.0.0.0", "--queueHost", "0.0.0.0", "--tableHost",
            "0.0.0.0", "--skipApiVersionCheck");
        emulator.WithDataVolume();
    })
    .AddBlobs("ObjectStorage");



var identityServerApi= builder.AddProject<Projects.IdentityServer_Api>("IdentityServerApi")
    .WithReference(identityServerDb)
    .WaitFor(identityServerDb);

var objectStorageApi = builder.AddProject<Projects.ObjectStorage_Api>("ObjectStorageApi")
    .WithReference(redisCache)
    .WithReference(table)
    .WithReference(blobs)
    .WithReference(identityServerApi)
    .WaitFor(redisCache)
    .WaitFor(table)
    .WaitFor(blobs)
    .WaitFor(identityServerApi);


var answerMeApi = builder.AddProject<Projects.AnswerMe_Api>("AnswerMeApi")
    .WithReference(answerMeDb)
    .WithReference(redisCache)
    .WithReference(blobs)
    .WithReference(objectStorageApi)
    .WithReference(identityServerApi)
    .WaitFor(answerMeDb)
    .WaitFor(redisCache)
    .WaitFor(blobs)
    .WaitFor(objectStorageApi)
    .WaitFor(identityServerApi);


var client = builder.AddProject<Projects.AnswerMe_Client>("AnswerMeClient")
    .WithReference(answerMeApi)
    .WithReference(identityServerApi)
    .WithReference(objectStorageApi)
    .WaitFor(answerMeApi)
    .WaitFor(identityServerApi)
    .WaitFor(objectStorageApi);




builder.Build().Run();