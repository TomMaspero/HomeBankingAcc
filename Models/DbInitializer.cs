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
                    new Client{FirstName="Kaladin", LastName="Stormblessed", Email="kaladin_bridge4@gmail.com", Password="syl" },
                    new Client{FirstName="Shallan", LastName="Davar", Email="sdavar@gmail.com", Password="pattern" },
                    new Client{FirstName="Adolin", LastName="Kholin", Email="best_dueler@gmail.com", Password="maya" }
                };

                context.Clients.AddRange(clients);
                context.SaveChanges();
            }
        }
    }
}
