using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace ClientLibrary.Helper
{
    public class CustomAuthenticationStateProvider(LocalStorageService localStorageService) : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal anonymous = new(new ClaimsIdentity());
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await localStorageService.GetToken();
            if (string.IsNullOrEmpty(token)) return new AuthenticationState(anonymous);

            var deserializeToken = Serializations.DeserializeObj<UserSession>(token);
            if (deserializeToken is null) return new AuthenticationState(anonymous);

            var getUserClaims = DecryptToken(deserializeToken.Token!);
            if (getUserClaims is null) return new AuthenticationState(anonymous);

            var claimsPrincipal = SetClaimPrincipal(getUserClaims);
            return new AuthenticationState(claimsPrincipal);
        }

        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if (userSession.Token is not null || userSession.RefreshToken is not null)
            {
                var serializeSession = Serializations.SerializeObj(userSession);
                await localStorageService.SetToken(serializeSession);
                var getUserClaims = DecryptToken(userSession.Token!);
                claimsPrincipal = SetClaimPrincipal(getUserClaims);
            }
            else
            {
                await localStorageService.RemoveToken();
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        private ClaimsPrincipal SetClaimPrincipal(CustomUserClaims claims)
        {
            if (claims.Email is null) return new ClaimsPrincipal();

            return new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, claims.Id),
                    new Claim(ClaimTypes.Name, claims.Name),
                    new Claim(ClaimTypes.Email, claims.Email),
                    new Claim(ClaimTypes.Role, claims.Role)
                }, "JwtAuth"));
        }

        private static CustomUserClaims DecryptToken(string jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken)) return new CustomUserClaims();

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwtToken);
            var userId = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
            var name = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value;
            var email = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value;
            var role = token.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value;
            return new CustomUserClaims(userId!, name!, email!, role!);
        }
    }
}