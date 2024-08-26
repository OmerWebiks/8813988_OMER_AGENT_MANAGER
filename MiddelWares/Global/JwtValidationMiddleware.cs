using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ManagementOfMossadAgentsAPI.MiddelWares.Global
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtValidationMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Headers {Authorization: Bearer ey37729ythkwaw4i}
            // Bearer ey37729ythkwaw4i
            // [Bearer,ey37729ythkwaw4i]
            try
            {
                string? BearerToken = context.Request.Headers["Authorization"].FirstOrDefault();
                string Token = BearerToken.Split(" ").Last();

                if (Token != null)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(
                        "ManagementOfMossadAgentsAPIManagementOfMossadAgentsAPI"
                    );
                    try
                    {
                        tokenHandler.ValidateToken(
                            Token,
                            new TokenValidationParameters
                            {
                                ValidateIssuerSigningKey = true,
                                IssuerSigningKey = new SymmetricSecurityKey(key),
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ClockSkew = TimeSpan.Zero
                            },
                            out SecurityToken validatedToken
                        );

                        var jwtToken = (JwtSecurityToken)validatedToken;

                        if (jwtToken.ValidTo < DateTime.UtcNow)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("Token has expired");
                            return;
                        }
                    }
                    catch
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Invalid Token");
                        return;
                    }
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized - Token is missing");
                    return;
                }
            }
            catch
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized - Token is missing");
                return;
            }
            await _next(context);
        }
    }
}
