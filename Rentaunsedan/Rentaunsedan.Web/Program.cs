using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Rentaunsedan.Data;
using Rentaunsedan.Data.Data;
using Rentaunsedan.Data.Repositories.CRM;
using Rentaunsedan.Data.Repositories.Users;
using Rentaunsedan.Entities.Entities.CRM;
using Rentaunsedan.Entities.Entities.CRM.TestPayload;
using Rentaunsedan.Services.Implementation;
using Rentaunsedan.Services.Implementation.CRM;
using Rentaunsedan.Services.Implementation.UsersService;
using Rentaunsedan.Services.Interfaces;
using Rentaunsedan.Services.Interfaces.UsersService;
using Rentaunsedan.Web.Components;
using Rentaunsedan.Web.Components.Account;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();      // <-- necesario para exponer los api controllers
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<UsersRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();

//builder.Services.AddScoped<CRMInboxRepository>();

// Repositorio COMPARTIDO entre endpoint y componentes (YA USA SQL)
builder.Services.AddSingleton<CRMInboxRepository>();

builder.Services.AddScoped<CRMInboxService>();
builder.Services.AddScoped<CRMxEquiposRepository>();
builder.Services.AddScoped<CRMxEquiposService>();
builder.Services.AddScoped<CRMxUsuariosRepository>();
builder.Services.AddScoped<CRMxUsuariosService>();
builder.Services.AddScoped<CRMxEquiposUsuariosRepository>();
builder.Services.AddScoped<CRMxEquiposUsuariosService>();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CustomClaimsPrincipalFactory>();


//builder.Services.AddAuthentication(options =>
//    {
//        options.DefaultScheme = IdentityConstants.ApplicationScheme;
//        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
//    })
//    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>{
    options.SignIn.RequireConfirmedAccount = true;
    // Fuerza email único y se utiliza en Edicion de usuarios del sistema (edit)
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddErrorDescriber<SpanishIdentityErrorDescriber>()   // Diccionario en español de errores de Identity
.AddDefaultTokenProviders();

// Fábrica para crear contextos EF por operación en el repositorio singleton
builder.Services.AddDbContextFactory<CrmInboxDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddDbContextFactory<ApplicationDbContext>(o =>
    o.UseSqlServer(connectionString));

//builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddSignInManager()
//    .AddDefaultTokenProviders();

//builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
//builder.Services.AddSingleton<IEmailSender, IdentityNoOpEmailSender>();

builder.Services.AddRazorPages();
builder.Services.AddRadzenComponents();
builder.Services.AddHttpContextAccessor();
// opcional: builder.Services.AddAntiforgery();  // usa valores por defecto

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.MapControllers();                   // <-- expone los ApiControllers
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();


app.MapRazorPages();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    string username = "mdiaz";
    string adminEmail = "admin@rentaunsedan.com";
    string adminPassword = "Abc123?"; // Solo por primer inicio

    // Crear rol "Administrador" si no existe
    var adminRoleExists = await roleManager.RoleExistsAsync("Administrador");
    if (!adminRoleExists)
    {
        await roleManager.CreateAsync(new IdentityRole("Administrador"));
    }

    // Crear rol "Usuario Sistema" si no existe
    var userRoleExists = await roleManager.RoleExistsAsync("Usuario Sistema");
    if (!userRoleExists)
    {
        await roleManager.CreateAsync(new IdentityRole("Usuario Sistema"));
    }

    // Verificar si el usuario administrador ya existe
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        var user = new ApplicationUser
        {
            UserName = username,
            Email = adminEmail,
            FullName = "Mario Diaz",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, "Administrador");
        }
    }
}

// Endpoint de prueba: simula recepción de un mensaje entrante
//app.MapPost("/webhooks/whatsapp/test", async (
//    TestInboundMessage dto,
//    CRMInboxService inbox,
//    CancellationToken ct) =>
//{
//    var threadId = $"{dto.BusinessPhoneId}:{dto.From}";
//    var text = dto.Text?.Trim() ?? string.Empty;
//    if (string.IsNullOrWhiteSpace(text))
//        return Results.BadRequest(new { ok = false, error = "Text vacío." });

//    var createdUtc = dto.Timestamp is long ts
//        ? DateTimeOffset.FromUnixTimeSeconds(ts).UtcDateTime
//        : DateTime.UtcNow;

//    await inbox.UpsertThreadAsync(
//        threadId: threadId,
//        businessPhoneId: dto.BusinessPhoneId,
//        customerPhone: dto.From,
//        lastMessageAtUtc: createdUtc,
//        customerName: dto.From,     // si no tienes nombre, usa el número
//        companyId: "DATA",          // pon lo que corresponda
//        lastPreview: text,
//        incrementUnread: false,
//        ct: ct
//    );

//    await inbox.AppendMessageAsync(threadId, SenderKind.Customer, dto.From, text, ct);

//    return Results.Ok(new { ok = true, threadId, createdUtc });
//})
//.DisableAntiforgery();



app.Run();