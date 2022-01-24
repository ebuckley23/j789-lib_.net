using IdentityModel;
using System.Security.Claims;

namespace J789.Library.Core.UnitTests.Fakes
{
    public class FakeClaimsPrincipal : ClaimsPrincipal
    {
        private readonly ClaimsIdentity _identity;
        public FakeClaimsPrincipal()
        {
            _identity = new ClaimsIdentity();
            AddIdentity(_identity);
        }

        public FakeClaimsPrincipal AsJwtClaim(
            string subject = default,
            string fName = default,
            string lName = default,
            string email = default,
            string name = default,
            string idProvider = default,
            string preferredUsername = default)
        {
            if (!string.IsNullOrWhiteSpace(subject))
                _identity.AddClaim(new Claim(JwtClaimTypes.Subject, subject));
            if (!string.IsNullOrWhiteSpace(fName))
                _identity.AddClaim(new Claim(JwtClaimTypes.GivenName, fName));
            if (!string.IsNullOrWhiteSpace(lName))
                _identity.AddClaim(new Claim(JwtClaimTypes.FamilyName, lName));
            if (!string.IsNullOrWhiteSpace(preferredUsername))
                _identity.AddClaim(new Claim(JwtClaimTypes.PreferredUserName, preferredUsername));
            if (!string.IsNullOrWhiteSpace(name))
                _identity.AddClaim(new Claim(JwtClaimTypes.Name, name));
            if (!string.IsNullOrWhiteSpace(email))
                _identity.AddClaim(new Claim(JwtClaimTypes.Email, email));
            if (!string.IsNullOrWhiteSpace(idProvider))
                _identity.AddClaim(new Claim(JwtClaimTypes.IdentityProvider, idProvider));
            return this;
        }

        public void AddCustomClaim(string claimProp, string claimValue)
        {
            _identity.AddClaim(new Claim(claimProp, claimValue));
        }
    }
}
