using HomeBankingAcc.Models;

namespace AccProject.Models
{
    public class DbInitializer
    {
        public static void Initialize(HomeBankingContext context)
        {
            if (!context.Clients.Any())
            {
                var clients = new Client[]
                {
                    new Client{FirstName="Nombre1", LastName="Apellido1", Email="email1@gmail.com", Password="pass1" },
                    new Client{FirstName="Nombre2", LastName="Apellido2", Email="email2@gmail.com", Password="pass2" },
                    new Client{FirstName="Nombre3", LastName="Apellido3", Email="email3@gmail.com", Password="pass3" }
                };

                context.Clients.AddRange(clients);
                context.SaveChanges();
            }
        }
    }
}
