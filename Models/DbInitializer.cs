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
                    new Client{FirstName="Percy", LastName="Jackson", Email="perseusposeidon@gmail.com", Password="1234" },
                    new Client{FirstName="Annabeth", LastName="Chase", Email="abethchase@gmail.com", Password="empState600" },
                    new Client{FirstName="Grover", LastName="Underwood", Email="pan4eva@gmail.com", Password="pan111" }
                };

                context.Clients.AddRange(clients);
                context.SaveChanges();
            }
        }
    }
}
