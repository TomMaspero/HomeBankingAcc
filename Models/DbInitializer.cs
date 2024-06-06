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

            if (!context.Accounts.Any())
            {
                Client kaladinClient = context.Clients.FirstOrDefault(cl => cl.Email == "kaladin_bridge4@gmail.com");
                if (kaladinClient != null)
                {
                    var kaladinAccounts = new Account[]
                    {
                        new Account{ClientId = kaladinClient.Id, CreationDate = DateTime.Now, Number = "VIN001", Balance = 5000},
                        new Account{ClientId = kaladinClient.Id, CreationDate = DateTime.Now, Number = "VIN002", Balance = 7500}
                    };
                    context.Accounts.AddRange(kaladinAccounts);
                    context.SaveChanges();
                }
            }
            if (!context.Transactions.Any())
            {
                var account1 = context.Accounts.FirstOrDefault(c => c.Number == "VIN001");
                if (account1 != null)
                {
                    var transactions = new Transaction[]
                    {
                        new Transaction { AccountId = account1.Id, Amount = 10000, Date = DateTime.Now.AddHours(-5), Description = "Transferencia recibida", Type=TransactionType.CREDIT},
                        new Transaction { AccountId = account1.Id, Amount = -2000, Date = DateTime.Now.AddHours(-6), Description = "Compra en MercadoLibre", Type=TransactionType.DEBIT},
                        new Transaction { AccountId = account1.Id, Amount = -3000, Date = DateTime.Now.AddHours(-7), Description = "Compra en Steam", Type=TransactionType.DEBIT}
                    };
                    context.Transactions.AddRange(transactions);
                    context.SaveChanges();
                }
            }
            if (!context.Loans.Any())
            {
                var loans = new Loan[]
                {
                    new Loan{Name="Hipotecario", MaxAmount=500000, Payments="12,24,36,48,60"},
                    new Loan{Name="Personal", MaxAmount=100000, Payments="6,12,24" },
                    new Loan{Name="Automotriz", MaxAmount=300000, Payments="6,12,24,36"}
                };
                context.Loans.AddRange(loans);
                context.SaveChanges();
            }
            if (!context.ClientLoans.Any())
            {
                var client1 = context.Clients.FirstOrDefault(c => c.Email == "kaladin_bridge4@gmail.com");
                if(client1 != null)
                {
                    var loan1 = context.Loans.FirstOrDefault(l => l.Name == "Hipotecario");
                    if(loan1 != null)
                    {
                        var clientLoan1 = new ClientLoan
                        {
                            Amount = 400000,
                            ClientId = client1.Id,
                            LoanId = loan1.Id,
                            Payments = "60"
                        };
                        context.ClientLoans.Add(clientLoan1);
                    }
                    var loan2 = context.Loans.FirstOrDefault(l => l.Name == "Personal");
                    if(loan2 != null)
                    {
                        var clientLoan2 = new ClientLoan
                        {
                            Amount = 50000,
                            ClientId = client1.Id,
                            LoanId = loan2.Id,
                            Payments = "12"
                        };
                        context.ClientLoans.Add(clientLoan2);
                    }
                    var loan3 = context.Loans.FirstOrDefault(l => l.Name == "Automotriz");
                    if(loan3 != null)
                    {
                        var clientLoan3 = new ClientLoan
                        {
                            Amount = 100000,
                            ClientId = client1.Id,
                            LoanId = loan3.Id,
                            Payments = "24"
                        };
                        context.ClientLoans.Add(clientLoan3);
                    }
                }
                context.SaveChanges();

            }
            if (!context.Cards.Any())
            {
                var client1 = context.Clients.FirstOrDefault(c => c.Email == "kaladin_bridge4@gmail.com");
                if(client1 != null)
                {
                    var cards = new Card[]
                    {
                        new Card
                        {
                            ClientId = client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.DEBIT,
                            Color = CardColor.GOLD,
                            Number = "3325-6745-7876-4445",
                            Cvv = 990,
                            FromDate = DateTime.Now,
                            ThruDate = DateTime.Now.AddYears(4),
                        },
                        new Card
                        {
                            ClientId = client1.Id,
                            CardHolder = client1.FirstName + " " + client1.LastName,
                            Type = CardType.CREDIT,
                            Color = CardColor.TITANIUM,
                            Number = "1111-1111-1111-1111",
                            Cvv = 001,
                            FromDate = DateTime.Now,
                            ThruDate= DateTime.Now.AddYears(5),
                        }
                    };
                    context.Cards.AddRange(cards);
                }
                context.SaveChanges();
            }
        }

    }
}
