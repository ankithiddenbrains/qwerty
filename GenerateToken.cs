using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tracer.API.Domain;
using Tracer.API.Helper.AppSetting;

namespace Tracer.API.Helper
{
    public class GenerateToken
    {
        #region variable
        /// <summary>
        /// The token key
        /// </summary>
        private readonly Token _tokenKey;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GenerateToken"/> class.
        /// </summary>
        /// <param name="tokenKey">The token key.</param>
        public GenerateToken(IOptions<Token> tokenKey)
        {
            _tokenKey = tokenKey.Value;
        }

        #endregion

        #region GenerateJSONWebToken
        /// <summary>
        /// Generates the json web token.
        /// </summary>
        /// <param name="userinfo">The userinfo.</param>
        /// <returns></returns>
        public string GenerateJSONWebToken(Login loginModel)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenKey.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,loginModel.Email),
                new Claim(JwtRegisteredClaimNames.Email,loginModel.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(

                issuer: _tokenKey.Issuer,
                audience: _tokenKey.Issuer,
                claims,
                expires: DateTime.Now.AddDays(3),
                signingCredentials: credentials);

            var encodeToken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodeToken;
        }
        #endregion

    }
}
