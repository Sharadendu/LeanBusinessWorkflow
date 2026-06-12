using LeanBusinessWorkflow.AI;
using LeanBusinessWorkflow.Core.Services;
using LeanBusinessWorkflow.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// === Lean Business Workflow Services ===
builder.Services.AddLeanBusinessWorkflowAI(builder.Configuration);

// Obsidian-style vault (configure path as needed)
var vaultPath = builder.Configuration["Obsidian:VaultPath"] 
    ?? Path.Combine(AppContext.BaseDirectory, "Data", "Vault");
builder.Services.AddSingleton(new ObsidianVaultService(vaultPath));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();