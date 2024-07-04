using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using AnswerMe.Client;
using AnswerMe.Client.Core;
using AnswerMe.Client.Core.DTOs.Base;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Application Settings
var settings = new AppSettings();
builder.Configuration.Bind("AppSettings", settings);
builder.Services.AddSingleton(settings);
builder.Services.AddCoreServices(settings);

await builder.Build().RunAsync();