using Reimbursement_Web_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace Reimbursement_Web_System.Controllers
{
    [SessionAuthorize]
    public class AdminController : Controller
    {
        public ActionResult Home()
        {
            return View(); //return user home view
        }

        public ActionResult PendingTickets()
        {

            Role role = (Role)Session["Role"]; //get role from session

            using (var context = new ReimbursementContext()) // initialize connection to dabase
            {
                int userId = (int)Session["UserId"]; //get user id from session
                List<Ticket> query = null;

                //identify what role of the user
                //then show only related ticket
                if (role.Equals(Role.Director)) 
                {
                    query = context.Ticket.Include(req => req.User)
                        .Where(s => s.Status == null || s.Status == Status.DirectorRejected) // show tickets that doesn't have status
                        .Select(p => p).ToList();
                }
                else if (role.Equals(Role.HSU))
                {
                    query = context.Ticket.Include(req => req.User)
                        .Where(s => s.Status == Status.DirectorApproved || s.Status == Status.HSURejected) // show tickets that director approved
                        .Select(p => p).ToList();
                }
                else if (role.Equals(Role.HR))
                {
                    query = context.Ticket.Include(req => req.User)
                        .Where(s => s.Status == Status.HSUApproved || s.Status == Status.HRRejected) // show tickets that HSU approved
                        .Select(p => p).ToList();
                }
                else if (role.Equals(Role.SDAS))
                {
                    query = context.Ticket.Include(req => req.User)
                        .Where(s => s.Status == Status.HRApproved || s.Status == Status.SDASRejected) // show tickets that HR approved
                        .Select(p => p).ToList();
                }
                else if (role.Equals(Role.Finance))
                {
                    query = context.Ticket.Include(req => req.User)
                        .Where(s => s.Status == Status.SDASApproved || s.Status == Status.FinanceRejected) // show tickets that SDAS approved
                        .Select(p => p).ToList();
                }
                ViewBag.pendingTickets = query;
                return View();
            }
        }

        public ActionResult ViewTicket(int crf)
        {
            using (var context = new ReimbursementContext())
            {
                //select the ticket and include related data
                var query = context.Ticket
                    .Include(req => req.User)
                    .Include(req => req.Reimbursement)
                    .Include(req => req.Medias)
                    .Where(s => s.CRF == crf)
                    .Select(p => p).SingleOrDefault();

                //select Reimbursement data
                var reimbursement = context.Reimbursement
                   .Where(s => s.Ticket.CRF == crf)
                   .OrderBy(s => s.Amount)
                   .Select(p => p).ToList();

                return View(query);
            }
        }

        public ActionResult ViewCompletedTicket(int crf)
        {
            using (var context = new ReimbursementContext())
            {
                //select the ticket and include related data
                var query = context.Ticket
                    .Include(req => req.User)
                    .Include(req => req.Reimbursement)
                    .Include(req => req.Medias)
                    .Where(s => s.CRF == crf)
                    .Select(p => p).SingleOrDefault();

                //select Reimbursement data
                var reimbursement = context.Reimbursement
                   .Where(s => s.Ticket.CRF == crf)
                   .OrderBy(s => s.Amount)
                   .Select(p => p).ToList();

                //store reimbursement data in viewbag
                ViewBag.reimbursements = reimbursement;

                return View(query);
            }
        }


        public ActionResult UpdateTicket(Ticket ticket, string command)
        {
            //remove username and password validation because it's not part of the ticket
            ModelState.Remove("User.Username");
            ModelState.Remove("User.Password");

            Role role = (Role)Session["Role"]; //get the role from the user

            if (ModelState.IsValid) // if the model is valid then proceed
            {
                using (var context = new ReimbursementContext()) //initialize database
                {

                    //get the reimbursement data from the database
                    var oldReimbursement = context.Reimbursement
                                .Where(s => s.TicketCRF == ticket.CRF)
                                .Select(p => p).ToList();

                    //remove all the reimbursement data
                    context.Reimbursement.RemoveRange(oldReimbursement);

                    //save to database
                    context.SaveChanges();

                    //get the Ticket data from the database
                    var oldobj = context.Ticket.Where(x => x.CRF == ticket.CRF).SingleOrDefault();

                    //modify the existing Reimbursement and readd the Reimbursement from the UI
                    oldobj.Reimbursement.AddRange(ticket.Reimbursement);

                    if (role.Equals(Role.Director))
                    {
                        if (command.Equals("Approve")) { ticket.Status = Status.DirectorApproved; } // if the user is a director marked the ticket as DirectorApproved
                        else { ticket.Status = Status.DirectorRejected; }
                    }
                    else if (role.Equals(Role.HSU))
                    {
                        if (command.Equals("Approve")) { ticket.Status = Status.HSUApproved; } // if the user is a director marked the ticket as HSUApproved
                        else { ticket.Status = Status.HSURejected; }
                    }
                    else if (role.Equals(Role.HR))
                    {
                        if (command.Equals("Approve")) { ticket.Status = Status.HRApproved; } // if the user is a director marked the ticket as HRApproved
                        else { ticket.Status = Status.HRRejected; }
                    }
                    else if (role.Equals(Role.SDAS))
                    {
                        if (command.Equals("Approve")) { ticket.Status = Status.SDASApproved; } // if the user is a director marked the ticket as SDASApproved
                        else { ticket.Status = Status.SDASRejected; }
                    }
                    else if (role.Equals(Role.Finance))
                    {
                        if (command.Equals("Approve"))
                        {
                            ticket.Status = Status.FinanceApproved;  // if the user is a director marked the ticket as FinanceApproved
                            ticket.DateCompleted = DateTime.Now; // modify the data completed to date today
                        }
                        else { ticket.Status = Status.FinanceRejected; }
                    }

                    context.Entry(oldobj).CurrentValues.SetValues(ticket); //change the old ticket to the new ticket
                    context.SaveChanges(); // save to database

                    //add the event in the notification
                    context.Notification.Add(new Notification { 
                        UserId = oldobj.User.Id,
                        message = ticket.Status.GetDisplayName() + " Ticket " + ticket.CRF
                    });
                    context.SaveChanges();

                }
                return RedirectToAction("PendingTickets", "Admin"); //redirect to PendingTickets function
            }
            else
            {
                return View("ViewTicket"); //return view for validation
            }
        }

        public ActionResult CompletedTickets()
        {
            using (var context = new ReimbursementContext())
            {
                int userId = (int)Session["UserId"]; //get the user id from session

                //select all tickets that are complete
                var query = context.Ticket.Include(req => req.User).Where(s => s.DateCompleted != null).Select(p => p).ToList();

                ViewBag.pendingTickets = query;
                return View();
            }
        }
    }
}