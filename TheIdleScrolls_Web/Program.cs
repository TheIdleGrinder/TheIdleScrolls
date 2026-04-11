using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TheIdleScrolls_Web;
using TheIdleScrolls_Web.CoreWrapper;
using TheIdleScrolls_Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<LocalStorageAccessor>();
builder.Services.AddSingleton<CoreWrapperModel>();
builder.Services.AddScoped<TooltipService>();

await builder.Build().RunAsync();
