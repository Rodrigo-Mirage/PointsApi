using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PointsApi.Models;
using PointsApi.Models.Results;

namespace PointsApi.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PointsController : ControllerBase
    {
        private readonly PointsContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        
        private readonly ILogger<ProductsController> _logger;

        public PointsController(UserManager<IdentityUser> userManager, ILogger<ProductsController> logger, PointsContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        [Route("GivePoints")]
        [Authorize(Roles = "Vendor")]
        public async Task<ActionResult<IEnumerable<Product>>> GivePoints(string userMail, double points)
        {
            string loggedMail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            System.Console.WriteLine(loggedMail);

            var loggedData = await _userManager.FindByEmailAsync(loggedMail);
            var userData = await _userManager.FindByEmailAsync(userMail);
            if(userData != null){

                var pointLog = new PointLog(){
                    UserID = userData.Id,
                    VendorId = loggedData.Id,
                    Value = points,
                    TimeStamp = DateTime.Now
                };

                var setLog = await _context.PointLogs.AddAsync(pointLog);
                if(setLog != null){
                    _context.SaveChanges();
                    return Ok( new Result(){
                        Message = "Os pontos foram creditados com sucesso",
                        Success = true,
                        Code = 200
                    });
                }
            }

            return BadRequest( new Result(){
                    Message = "Não foi possivel entregar os pontos a essa conta.",
                    Success = false,
                    Code = 300
                });
            

        }
        
        [HttpGet]
        [Route("GetProduct")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProduct(int productId)
        {
            var product = _context.Products.Where(e=>e.ProductId == productId).FirstOrDefault();
            if(product != null){
                string loggedMail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var loggedData = await _userManager.FindByEmailAsync(loggedMail);

                if(loggedData != null){
                    if(product.Value >getUserPoints(loggedData.Id)){
                        return BadRequest( new Result(){
                            Message = "Saldo Insuficiente",
                            Success = false,
                            Code = 300
                        });
                    }else{
                        var purchase = _context.PointLogs.Add(new PointLog(){UserID = loggedData.Id, VendorId = "",ProductId = product.ProductId, Value = product.Value, TimeStamp = DateTime.Now});
                        if(purchase != null){
                            _context.SaveChanges();
                            return Ok( new Result(){
                                Message = "Compra efetuada com sucesso",
                                Success = true,
                                Code = 200
                            });
                        }else{
                            return BadRequest( new Result(){
                                Message = "Não foi possivel efetuar a compra",
                                Success = false,
                                Code = 300
                            });

                        }
                    }
                    
                }
            }
            
            return BadRequest( new Result(){
                Message = "Não foi possivel encontrar o produto",
                Success = false,
                Code = 300
            });
        }

        [HttpGet]
        [Route("GetHistory")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<IEnumerable<Product>>> GetHistory()
        {
            string loggedMail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var loggedData = await _userManager.FindByEmailAsync(loggedMail);

            if(loggedData != null){
                var history = _context.PointLogs.Where(e=>e.UserID == loggedData.Id).ToList();
                return Ok( new HistoryResult{
                    Total = getUserPoints(loggedData.Id),
                    HistoryItens = history
                });
            }
            
            return BadRequest( new Result(){
                Message = "Não foi possivel encontrar o histórico",
                Success = false,
                Code = 300
            });
        }
    
        private double getUserPoints(string userId){

            var purchases = _context.PointLogs.Where(e=>e.UserID == userId && e.ProductId != 0).ToArray();
            var given     = _context.PointLogs.Where(e=>e.UserID == userId && e.VendorId != "").ToArray();

            var value = 0.00;
            for (int i = 0; i < purchases.Count(); i++)
            {
                value -= purchases[i].Value;
            }

            for (int i = 0; i < given.Count(); i++)
            {
                value += given[i].Value;
            }

            return value;
        }
    
    }
}
