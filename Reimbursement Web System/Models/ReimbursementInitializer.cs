using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Reimbursement_Web_System.Models
{
    public class ReimbursementInitializer<T> : DropCreateDatabaseIfModelChanges<ReimbursementContext>
    {
        private static Random rnd = new Random();

        protected override void Seed(ReimbursementContext context)
        {
            //set the ticket id to start with 1000
            context.Database.ExecuteSqlCommand("DBCC CHECKIDENT ('Tickets', RESEED, 1000)");

            //seed database
            IList<User> defaultUsers = new List<User>();

            defaultUsers.Add(new User() { Username = "StandardUserOne", Password = UtilityClass.GetHash("StandardUserOne"), FirstName = "Melissa", LastName = "Rivas", Role = Role.Standard });
            defaultUsers.Add(new User() { Username = "StandardUserTwo", Password = UtilityClass.GetHash("StandardUserTwo"), FirstName = "Cassia", LastName = "Hawes", Role = Role.Standard });
            defaultUsers.Add(new User() { Username = "Director", Password = UtilityClass.GetHash("Director"), FirstName = "Alanna", LastName = "Frost", Role = Role.Director });
            defaultUsers.Add(new User() { Username = "HSU", Password = UtilityClass.GetHash("HSU"), FirstName = "Zelda", LastName = "Glisson", Role = Role.HSU });
            defaultUsers.Add(new User() { Username = "HR", Password = UtilityClass.GetHash("HR"), FirstName = "Carly", LastName = "Schultz", Role = Role.HR });
            defaultUsers.Add(new User() { Username = "SDAS", Password = UtilityClass.GetHash("SDAS"), FirstName = "Gilbert", LastName = "Pearce", Role = Role.SDAS });
            defaultUsers.Add(new User() { Username = "Finance", Password = UtilityClass.GetHash("Finance"), FirstName = "Andrew", LastName = "Willis", Role = Role.Finance });

            context.User.AddRange(defaultUsers);

            IList<Ticket> defaulTicket = new List<Ticket>();

            for (int i = 0; i < 2; i++)
            {
                defaulTicket.Add(generateTicket(defaultUsers[0], DateTime.Now, null, null));
            }

            for (int i = 0; i < 2; i++)
            {
                defaulTicket.Add(generateTicket(defaultUsers[0], DateTime.Now, null, Status.DirectorApproved));
            }

            for (int i = 0; i < 2; i++)
            {
                defaulTicket.Add(generateTicket(defaultUsers[0], DateTime.Now, null, Status.HRApproved));
            }

            for (int i = 0; i < 2; i++)
            {
                defaulTicket.Add(generateTicket(defaultUsers[0], DateTime.Now, null, Status.HSUApproved));
            }

            for (int i = 0; i < 2; i++)
            {
                defaulTicket.Add(generateTicket(defaultUsers[0], DateTime.Now, null, Status.SDASApproved));
            }

            for (int i = 0; i < 15; i++)
            {
                defaulTicket.Add(generateTicket(defaultUsers[0], DateTime.Now, DateTime.Now, Status.FinanceApproved));
            }

            context.Ticket.AddRange(defaulTicket);

            for (int i = 0; i < defaulTicket.Count(); i++)
            {
                context.Reimbursement.AddRange(generateMultipleReimbursement(defaulTicket[i]));
            }

            base.Seed(context);
        }

        private Ticket generateTicket(User user, DateTime dateFiled, DateTime? dateCompleted, Status? status) {
            Ticket ticket = new Ticket
            {
                User = user,
                DateFiled = dateFiled,
                Purpose = "Office Supplies Reimbursement",
                Office = "Makati City",
                Status = status,
                DateCompleted = dateCompleted
            };

            return ticket;
        }

        private IList<Reimbursement> generateMultipleReimbursement(Ticket ticket) {
            IList<Reimbursement> lsReimbursement = new List<Reimbursement>();
            lsReimbursement.Add(generateReimbursement(ticket, 0, true));
            lsReimbursement.Add(generateReimbursement(ticket, 0, true));
            lsReimbursement.Add(generateReimbursement(ticket, 0, true));
            lsReimbursement.Add(generateReimbursement(ticket, 0, false));
            lsReimbursement.Add(generateReimbursement(ticket, 0, false));
            return lsReimbursement;
        }

        private Reimbursement generateReimbursement(Ticket ticket, int Type, bool status) {
            Reimbursement reimbursement = new Reimbursement
            {
                Ticket = ticket,
                Date = ticket.DateFiled,
                NatureOfExpenditure = "Office Supply X",
                Amount = GetRandom(), //get random amount
                Status = status
            };

            return reimbursement;
        }

        public static int GetRandom()
        {
            return rnd.Next(500, 1000);
        }
    }
}