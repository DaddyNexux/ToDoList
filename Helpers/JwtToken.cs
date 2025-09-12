using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ToDoList.Helpers;

public static class JwtToken
{
    private static SymmetricSecurityKey _key;

    public static string GenToken(Guid userId, string role, string issuer, int days, string secretKey)
    {
        if (string.IsNullOrWhiteSpace(secretKey))
            throw new ArgumentException("JWT secret key is missing.", nameof(secretKey));

        byte[] bytes = Encoding.ASCII.GetBytes(secretKey);
        var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Subject = new ClaimsIdentity(new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Role, role)
    }),
            Expires = DateTime.UtcNow.AddDays(days),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(bytes),
                SecurityAlgorithms.HmacSha512Signature)
        };

        SecurityToken token = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
        return jwtSecurityTokenHandler.WriteToken(token);
    }

}
