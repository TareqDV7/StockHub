using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using StockHub.Web.Core.Models;
using System.Security.Claims;

namespace StockHub.Web.Data
{
    public class ApplicationUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            IHttpContextAccessor httpContextAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (user.BeneficiaryId.HasValue)
            {
                identity.AddClaim(new Claim("BeneficiaryId", user.BeneficiaryId.Value.ToString()));
            }

            return identity;
        }
    }
}