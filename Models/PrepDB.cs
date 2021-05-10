using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using PointsApi.Controllers;
using System.Threading.Tasks;
using System;
using PointsApi.Models.Requests;
using Microsoft.AspNetCore.Identity;

namespace PointsApi.Models{

    public static class PrepDB{
        
        public static void prepPopulation(IApplicationBuilder app){
            using (var serviceScope = app.ApplicationServices.CreateScope()){
                SeedData(serviceScope.ServiceProvider.GetService<PointsContext>());

                CreateBaseRoles(serviceScope.ServiceProvider);
                CreateDummyUsers(serviceScope.ServiceProvider);
            }
        }

        public static void SeedData(PointsContext context){
            System.Console.WriteLine("applying migrations ...");
            context.Database.Migrate();

            if(!context.Products.Any()){
                System.Console.WriteLine("applying seed ...");

                context.Products.AddRange(
                    new Product(){ Name = "Bicicleta", Value = 50000.00 , Photo_Url = "https://decathlonpro.vteximg.com.br/arquivos/ids/2439568-1000-1000/--st-100-red-l1.jpg?v=637110782091530000"},
                    new Product(){ Name = "Secador" , Value = 30000.00 , Photo_Url = "https://www.casasbahia-imagens.com.br/Control/ArquivoExibir.aspx?IdArquivo=1197698150"},
                    new Product(){ Name = "Mouse Gamer" , Value = 15000.00 , Photo_Url = "https://images8.kabum.com.br/produtos/fotos/112948/mouse-gamer-logitech-g203-rgb-lightsync-6-botoes-8000-dpi-preto-910-005793_1612880277_gg.jpg"}
                );
                

                context.SaveChanges();                
            }else{
                System.Console.WriteLine("data already exists");
            }


        }

        private static void CreateBaseRoles(IServiceProvider serviceProvider)
        {
           var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            string[] rolesNames = { "Admin", "User", "Vendor" };
            System.Console.WriteLine("setting rules ...");
            foreach(var namesRole in rolesNames)
            {
                var roleExist = roleManager.RoleExistsAsync(namesRole).Result ;
                if(!roleExist)
                {
                    var result = roleManager.CreateAsync(new IdentityRole(namesRole)).Result;
                }
            }
        }


        ///Para fins de teste
        private static void CreateDummyUsers(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var roleExist =  roleManager.RoleExistsAsync("Vendor").Result;
            var vendorId1 = "";
            var vendorId2 = "";
            if(roleExist)
            {
                var user = new UserRegRequest(){
                    UserName = "Magazine",
                    Email = "maga@zine.com",
                    Password = "Magaz1ne!"
                };

                var existingUser =  userManager.FindByEmailAsync(user.Email).Result;

                if(existingUser == null){
                    
                    var newUser = new IdentityUser(){ Email = user.Email, UserName = user.UserName};
                    var isCreated =  userManager.CreateAsync(newUser,user.Password).Result;
                    var existing =  userManager.FindByEmailAsync(user.Email).Result;
                    var result =  userManager.AddToRoleAsync(existing,"Vendor").Result;
                    vendorId1 = existing.Id;
                }
                else{
                    vendorId1 = existingUser.Id;
                }


                var user2 = new UserRegRequest(){
                    UserName = "Cem_Pontos",
                    Email = "cem@pontos.com",
                    Password = "100Pontos."
                };

                var existingUser2 = userManager.FindByEmailAsync(user2.Email).Result;

                if(existingUser2 == null){
                    
                    var newUser = new IdentityUser(){ Email = user2.Email, UserName = user2.UserName};
                    var isCreated =  userManager.CreateAsync(newUser,user2.Password).Result;
                    var existing =  userManager.FindByEmailAsync(user2.Email).Result;
                    var result =  userManager.AddToRoleAsync(existing,"Vendor").Result;
                    vendorId2 = existing.Id;
                }
                else{
                    vendorId2 = existingUser.Id;
                }

            }


            roleExist =  roleManager.RoleExistsAsync("User").Result;
            if(roleExist)
            {
                var user3 = new UserRegRequest(){
                    UserName = "Joe_Natan",
                    Email = "joe@natan.com",
                    Password = "j0En@t@n"
                };

                var existingUser =  userManager.FindByEmailAsync(user3.Email).Result;
                var userId = "";
                if(existingUser == null){
                    
                    var newUser = new IdentityUser(){ Email = user3.Email, UserName = user3.UserName};
                    var isCreated =  userManager.CreateAsync(newUser,user3.Password).Result;
                    var existing =  userManager.FindByEmailAsync(user3.Email).Result;
                    var result =  userManager.AddToRoleAsync(existing,"User").Result;
                    userId = existing.Id;

                    var _context = serviceProvider.GetService<PointsContext>();
                    //adiciona endereço

                    //adiciona compras para o usuario

                    //adiciona pontos para o usuario
                    _context.PointLogs.AddRange(
                        new PointLog(){UserID = userId, VendorId = vendorId1, Value = 2500.00, TimeStamp = new DateTime(2019,3,25)},
                        new PointLog(){UserID = userId, VendorId = vendorId2, Value = 420.00, TimeStamp = new DateTime(2019,9,15)},
                        new PointLog(){UserID = userId, VendorId = vendorId2, Value = 310.00, TimeStamp = new DateTime(2020,10,2)},
                        new PointLog(){UserID = userId, VendorId = vendorId1, Value = 1500.00, TimeStamp = new DateTime(2021,01,5)}
                    );

                    _context.SaveChanges();

                }

            }
        }
    
    }
}