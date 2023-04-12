using GeekStore.IdentityServer.Configuration;
using GeekStore.IdentityServer.Model;
using GeekStore.IdentityServer.Model.Context;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GeekStore.IdentityServer.Initializer;

public class DbInitializer : IDbInitializer
{
    private readonly DataContext _context;
    private readonly UserManager<ApplicationUser> _user;
    private readonly RoleManager<IdentityRole> _role;

    public DbInitializer(DataContext context, UserManager<ApplicationUser> user, RoleManager<IdentityRole> role)
    {
        _context = context;
        _user = user;
        _role = role;
    }

    public void Initialize()
    {
        if (_role.FindByNameAsync(IdentityConfiguration.Admin).Result != null) return;
        _role.CreateAsync(new IdentityRole(IdentityConfiguration.Admin)).GetAwaiter().GetResult();
        _role.CreateAsync(new IdentityRole(IdentityConfiguration.Client)).GetAwaiter().GetResult();

        ApplicationUser admin = new ApplicationUser()
        {
            UserName = "luiz-admin",
            Email = "luiz-admin@lajus.com.br",
            EmailConfirmed = true,
            PhoneNumber = "+55 (49) 91324-5214",
            FirstName = "Luiz",
            LastName = "Admin"
        };

        _user.CreateAsync(admin, "Lajus123$").GetAwaiter().GetResult();
        _user.AddToRoleAsync(admin, IdentityConfiguration.Admin).GetAwaiter().GetResult();

        var adminClaims = _user.AddClaimsAsync(admin, new Claim[]
        {
            new Claim(JwtClaimTypes.Name, $"{admin.FirstName} {admin.LastName}"),
            new Claim(JwtClaimTypes.GivenName, admin.FirstName),
            new Claim(JwtClaimTypes.FamilyName, admin.LastName),
            new Claim(JwtClaimTypes.Role, IdentityConfiguration.Admin)
        }).Result;        
        
        ApplicationUser client = new ApplicationUser()
        {
            UserName = "luiz-client",
            Email = "luiz-client@lajus.com.br",
            EmailConfirmed = true,
            PhoneNumber = "+55 (49) 91324-5214",
            FirstName = "Luiz",
            LastName = "Client"
        };

        _user.CreateAsync(client, "Lajus123$").GetAwaiter().GetResult();
        _user.AddToRoleAsync(client, IdentityConfiguration.Client).GetAwaiter().GetResult();

        var clientClaims = _user.AddClaimsAsync(client, new Claim[]
        {
            new Claim(JwtClaimTypes.Name, $"{client.FirstName} {client.LastName}"),
            new Claim(JwtClaimTypes.GivenName, client.FirstName),
            new Claim(JwtClaimTypes.FamilyName, client.LastName),
            new Claim(JwtClaimTypes.Role, IdentityConfiguration.Client)
        }).Result;
    }
}
