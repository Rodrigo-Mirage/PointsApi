using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PointsApi.Models;
using PointsApi.Options;
using Microsoft.Extensions.Options;
using PointsApi.Models.Requests;
using PointsApi.Models.Results;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace PointsApi.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtSettings _jwtSettings;
        private readonly PointsContext _context;

        public AuthController(UserManager<IdentityUser> userManager, JwtSettings jwtSettings,PointsContext context){

            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _context = context;

        }
        
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegRequest user){

            if(ModelState.IsValid){
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if(existingUser != null){
                    return BadRequest( new UserRegResult(){
                        Errors = new List<string>(){
                            "E-mail Inválido"
                        },
                        Success = false
                    });
                }

                var newUser = new IdentityUser(){ Email = user.Email, UserName = user.UserName};
                var isCreated = await _userManager.CreateAsync(newUser,user.Password);

                if(isCreated.Succeeded){
                    var existing = await _userManager.FindByEmailAsync(user.Email);
                    var setRule = await _userManager.AddToRoleAsync(existing,"User");

                    var jwtToken = await TokenGen(newUser);
                    return Ok(
                        new UserRegResult(){
                            Success = true,
                            Token = jwtToken
                        }
                    );
                }else{
                    
                    return BadRequest( new UserRegResult(){
                        Errors = new List<string>(){
                            "Não foi possivel criar o usuário"
                        },
                        Success = false
                    });
                }


            }

            return BadRequest( new UserRegResult(){
                Errors = new List<string>(){
                    "Formulário Inválido"
                },
                Success = false
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest user){

            if(ModelState.IsValid){ 
                
                var existingUser = await _userManager.FindByEmailAsync(user.Email);

                if(existingUser == null){
                    return BadRequest( new UserRegResult(){
                        Errors = new List<string>(){
                            "Login Inválido ou Não encontrado"
                        },
                        Success = false
                    });
                }

                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

                if(!isCorrect){
                    return BadRequest( new UserRegResult(){
                        Errors = new List<string>(){
                            "Login Inválido ou Não encontrado"
                        },
                        Success = false
                    });
                }
                
                var jwtToken = await TokenGen(existingUser);
                _context.SaveChanges();
                return Ok(
                    new UserRegResult(){
                        Success = true,
                        Token = jwtToken
                    }
                );

            }

            return BadRequest( new UserRegResult(){
                Errors = new List<string>(){
                    "Formulário Inválido"
                },
                Success = false
            });
        }
        private async Task<string> TokenGen (IdentityUser user){
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            
            var roles = await _userManager.GetRolesAsync(user);
            AddRolesToClaims(claims, roles);

            var TokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(6),
                SigningCredentials = new SigningCredentials( new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(TokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;

        }

        
        private void AddRolesToClaims(List<Claim> claims, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                var roleClaim = new Claim(ClaimTypes.Role, role);
                claims.Add(roleClaim);
            }
        }
    }
}
