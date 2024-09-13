using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServerLibrary.Data;
using ServerLibrary.Helper;
using ServerLibrary.Repositories.Contracts;

namespace ServerLibrary.Repositories.Implementations
{
    public class UserAccountRepository(IOptions<JwtSection> config, AppDbContext appDbContext) : IUserAccount
    {
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            if (user is null) return new GeneralResponse(false, "Model is empty");

            var checkUser = await FindUserByEmail(user.Email!);

            if (checkUser != null) return new GeneralResponse(false, "User already exists");

            var newUser = await AddToDabase(new ApplicationUser
            {
                Fullname = user.Fullname,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password),
            });

            var checkAdminRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(x => x.Name!.Equals(Constants.Admin));

            if (checkAdminRole is null)
            {
                var createAdminRole = await AddToDabase(new SystemRole { Name = Constants.Admin });
                await AddToDabase(new UserRole { RoleId = createAdminRole.Id, UserId = newUser.Id });
                return new GeneralResponse(true, "User created successfully");
            }

            var checkUserRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(x => x.Name!.Equals(Constants.User));
            SystemRole response = new();
            if (checkUserRole is null)
            {
                response = await AddToDabase(new SystemRole { Name = Constants.User });
                await AddToDabase(new UserRole { RoleId = response.Id, UserId = newUser.Id });
            }
            else
            {
                await AddToDabase(new UserRole { RoleId = checkUserRole.Id, UserId = newUser.Id });
            }

            return new GeneralResponse(true, "User created successfully");
        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
            if (user is null) return new LoginResponse(false, "Model is empty");

            var applicationUser = await FindUserByEmail(user.Email!);
            if (applicationUser is null) return new LoginResponse(false, "User not found");

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(user.Password!, applicationUser.Password!))
                return new LoginResponse(false, "Email or password is incorrect");

            var userRole = await FindUserRole(applicationUser.Id);
            if (userRole is null) return new LoginResponse(false, "User role not found");

            var role = await FindRole(userRole.RoleId);
            if (role is null) return new LoginResponse(false, "User role not found");

            string jwtToken = GenerateToken(applicationUser, role.Name!);
            string refreshToken = GenerateRefreshToken();

            var findUserToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(x => x.UserId!.Equals(applicationUser.Id));
            if (findUserToken is not null)
            {
                findUserToken.Token = refreshToken;
                await appDbContext.SaveChangesAsync();
            }
            else
            {
                await AddToDabase(new RefreshTokenInfo { Token = refreshToken, UserId = applicationUser.Id });
            }

            return new LoginResponse(true, "Login successful", jwtToken, refreshToken);
        }

        private string GenerateToken(ApplicationUser user, string role)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Fullname!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role),
            };

            var token = new JwtSecurityToken(
                config.Value.Issuer,
                config.Value.Audience,
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserRole?> FindUserRole(int userId) => await appDbContext.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);

        private async Task<SystemRole?> FindRole(int roleId) => await appDbContext.SystemRoles.FirstOrDefaultAsync(x => x.Id == roleId);

        private static string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private async Task<ApplicationUser?> FindUserByEmail(string email) => await appDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Email!.ToLower()!.Equals(email!.ToLower()));

        private async Task<T> AddToDabase<T>(T model)
        {
            var result = appDbContext.Add(model!);
            await appDbContext.SaveChangesAsync();
            return (T)result.Entity;
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            if (token is null) return new LoginResponse(false, "Model is empty");

            var findToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(x => x.Token!.Equals(token.Token));

            if (findToken is null) return new LoginResponse(false, "Refresh token is required");

            var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == findToken.UserId);
            if (user is null) return new LoginResponse(false, "User not found");

            var userRole = await FindUserRole(user.Id);
            var role = await FindRole(userRole!.RoleId);

            string jwtToken = GenerateToken(user, role!.Name!);
            string refreshToken = GenerateRefreshToken();

            var updateRefreshToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(x => x.UserId!.Equals(user.Id));
            if (updateRefreshToken is null) return new LoginResponse(false, "Refresh token not found");

            updateRefreshToken.Token = refreshToken;
            appDbContext.RefreshTokenInfos.Update(updateRefreshToken);
            await appDbContext.SaveChangesAsync();
            return new LoginResponse(true, "Token refreshed successfully", jwtToken, refreshToken);
        }
    }
}