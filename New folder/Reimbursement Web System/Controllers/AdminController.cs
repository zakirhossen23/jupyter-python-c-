using Reimbursement_Web_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

using System.IO;
using System.Diagnostics;


namespace Reimbursement_Web_System.Controllers
{
    [SessionAuthorize]
    public class AdminController : Controller
    {
        public ActionResult Home()
        {
            return View(); //return user home view
        }
        public static string getdatefunc(DateTime value)
        {
            return value.Month + "/" + value.Day + "/" + value.Year;
        }
        public ActionResult PendingTicketsSearch( string search)
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
                       .AsEnumerable().Where(s => s.Status == null).Where(s=>s.CRF.ToString().Contains(search) || s.User.FirstName.ToString().Contains(search) || s.User.LastName.ToString().Contains(search) || s.DateFiled.ToString("MM/dd/yyyy").Contains(search)) // show tickets that doesn't have status
                        .ToList();
                }
                else if (role.Equals(Role.HSU))
                {
                    query = context.Ticket.Include(req => req.User)
                        .AsEnumerable().Where(s => s.Status == Status.DirectorApproved || s.Status == Status.DirectorRejected).Where(s => s.CRF.ToString().Contains(search) || s.User.FirstName.ToString().Contains(search) || s.User.LastName.ToString().Contains(search) || s.DateFiled.ToString("MM/dd/yyyy").Contains(search)) // show tickets that director approved
                        .Select(p => p).ToList();
                }
                else if (role.Equals(Role.HR))
                {
                    query = context.Ticket.Include(req => req.User)
                       .AsEnumerable().Where(s => s.Status == Status.HSUApproved || s.Status == Status.HSURejected).Where(s => s.CRF.ToString().Contains(search) || s.User.FirstName.ToString().Contains(search) || s.User.LastName.ToString().Contains(search) || s.DateFiled.ToString("MM/dd/yyyy").Contains(search)) // show tickets that HSU approved
                        .Select(p => p).ToList();
                }
                else if (role.Equals(Role.SDAS))
                {
                    query = context.Ticket.Include(req => req.User)
                       .AsEnumerable().Where(s => s.Status == Status.HRApproved || s.Status == Status.HRRejected).Where(s => s.CRF.ToString().Contains(search) || s.User.FirstName.ToString().Contains(search) || s.User.LastName.ToString().Contains(search) || s.DateFiled.ToString("MM/dd/yyyy").Contains(search)) // show tickets that HR approved
                        .Select(p => p).ToList();
                }
                else if (role.Equals(Role.Finance))
                {
                    query = context.Ticket.Include(req => req.User)
                         .AsEnumerable().Where(s => s.Status == Status.SDASApproved || s.Status == Status.SDASRejected).Where(s => s.CRF.ToString().Contains(search) || s.User.FirstName.ToString().Contains(search) || s.User.LastName.ToString().Contains(search) || s.DateFiled.ToString("MM/dd/yyyy").Contains(search)) // show tickets that SDAS approved
                        .Select(p => p).ToList();
                }

                ViewBag.pendingTickets = query;
                return View("PendingTickets");
            }
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
                        .Where(s => s.Status == null) // show tickets that doesn't have status
                        .Select(p => p).ToList();
                }
                else if (role.Equals(Role.HSU))
                {
                    query = context.Ticket.Include(req => req.User)
                        .Where(s => s.Status == Status.DirectorApproved || s.Status == Status.DirectorRejected) // show tickets that director approved
                        .Select(p => p).ToList();
                }
                else if (role.Equals(Role.HR))
                {
                    query = context.Ticket.Include(req => req.User)
                        .Where(s => s.Status == Status.HSUApproved || s.Status == Status.HSURejected) // show tickets that HSU approved
                        .Select(p => p).ToList();
                }
                else if (role.Equals(Role.SDAS))
                {
                    query = context.Ticket.Include(req => req.User)
                        .Where(s => s.Status == Status.HRApproved || s.Status == Status.HRRejected) // show tickets that HR approved
                        .Select(p => p).ToList();
                }
                else if (role.Equals(Role.Finance))
                {
                    query = context.Ticket.Include(req => req.User)
                        .Where(s => s.Status == Status.SDASApproved || s.Status == Status.SDASRejected) // show tickets that SDAS approved
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
                    .Include(req => req.Medias)//mediasssssssssssssssssssssssssssssssssssssssssssss
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
            if (command == "SaveImage")
            {
                using (var context = new ReimbursementContext()) //initialize database
                {


                    var dbTicket = context.Ticket
                    .Where(x => x.CRF == ticket.CRF)
                    .SingleOrDefault();

                    var oldMedia = context.Media
                            .Where(s => s.TicketCRF == ticket.CRF)
                            .Select(p => p).ToList();


                    if (ticket.Medias != null && ticket.Medias.Count > 0)
                    {
                        //readd all media except id == 0 which is deleted
                        dbTicket.Medias.AddRange(ticket.Medias.Where(x => x.Id != 0));
                    }
                    //same code in create. please refer in line #108
                    if (ticket.ImagesUpload.Count() != 0)
                    {
                        string uploadDir = "Ticket_Images";
                        string fileName;
                        foreach (var rec in ticket.ImagesUpload)
                        {
                            if (rec != null)
                            {
                                fileName = Path.GetFileName(rec.FileName);
                                fileName = fileName.Substring(0, fileName.IndexOf('.')) + "_" + DateTime.Now.Millisecond + "-" + DateTime.Now.Second + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Hour + "." + fileName.Substring(fileName.IndexOf('.') + 1);
                                rec.SaveAs(Path.Combine(Server.MapPath("~/" + uploadDir), fileName));
                                dbTicket.Medias.Add(new Media
                                {
                                    ImagePath = "/" + uploadDir + "/" + fileName
                                }); ;
                            }
                        }


                        //update the existing ticket to the new ticket
                        context.Entry(dbTicket).CurrentValues.SetValues(ticket);

                        //save in database
                        context.SaveChanges();
                    }

                }
                return View("Pending");
            }
            else if (ModelState.IsValid) // if the model is valid then proceed
            {

                //if all Reimbursement is cancelled then marked the ticket as rejected
                if (ticket.Reimbursement.Count(x => x.Status == false) == ticket.Reimbursement.Count())
                {
                    //below dont remove
                    command = "Rejected";
                }

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
                        if (command.Equals("Approve")) { ticket.Status = Status.DirectorApproved; ticket.DirectorStatus = "Approved"; ticket.HSUStatus = "active"; } // if the user is a director marked the ticket as DirectorApproved
                        else { ticket.Status = Status.DirectorRejected; ticket.DirectorStatus = "Rejected"; ticket.HSUStatus = "active"; }
                    }
                    else if (role.Equals(Role.HSU))
                    {
                        //get the existing ticket
                        var dbTicket = context.Ticket
                            .Where(x => x.CRF == ticket.CRF)
                            .SingleOrDefault();

                        //same code in create. please refer in line #108
                        if (ticket.ImagesUpload.Count() != 0)
                        {
                            string uploadDir = "Ticket_Images";
                            string fileName;
                            foreach (var rec in ticket.ImagesUpload)
                            {
                                if (rec != null)
                                {
                                    fileName = Path.GetFileName(rec.FileName);
                                    fileName = fileName.Substring(0, fileName.IndexOf('.')) + "_" + DateTime.Now.Millisecond + "-" + DateTime.Now.Second + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Hour + "." + fileName.Substring(fileName.IndexOf('.') + 1);
                                    rec.SaveAs(Path.Combine(Server.MapPath("~/" + uploadDir), fileName));
                                    dbTicket.Medias.Add(new Media
                                    {
                                        ImagePath = "/" + uploadDir + "/" + fileName
                                    }); ;
                                }
                            }
                        }

                        //update the existing ticket to the new ticket
                        context.Entry(dbTicket).CurrentValues.SetValues(ticket);

                        //save in database
                        context.SaveChanges();
                        if (command.Equals("Approve")) { ticket.Status = Status.HSUApproved; ticket.HSUStatus = "Approved"; ticket.HRStatus = "active"; } // if the user is a director marked the ticket as HSUApproved
                        else
                        {
                            //below dont remove
                            ticket.Status = Status.HSURejected;
                            ticket.HSUStatus = "Rejected"; ticket.HRStatus = "active";
                        }
                    }
                    else if (role.Equals(Role.HR))
                    {


                        if (command.Equals("Approve")) { ticket.Status = Status.HRApproved; ticket.HRStatus = "Approved"; ticket.SDASStatus = "active"; } // if the user is a director marked the ticket as HRApproved
                        else { ticket.Status = Status.HRRejected; ticket.HRStatus = "Rejected"; ticket.SDASStatus = "active"; }
                    }
                    else if (role.Equals(Role.SDAS))
                    {
                        if (command.Equals("Approve")) { ticket.Status = Status.SDASApproved; ticket.SDASStatus = "Approved"; ticket.FinanceStatus = "active"; } // if the user is a director marked the ticket as SDASApproved
                        else { ticket.Status = Status.SDASRejected; ticket.SDASStatus = "Rejected"; ticket.FinanceStatus = "active"; }
                    }
                    else if (role.Equals(Role.Finance))
                    {
                        if (command.Equals("Approve"))
                        {
                            ticket.Status = Status.FinanceApproved;  // if the user is a director marked the ticket as FinanceApproved
                            ticket.FinanceStatus = "Approved";
                            ticket.DateCompleted = DateTime.Now; // modify the data completed to date today
                        }
                        else { ticket.Status = Status.FinanceRejected; ticket.FinanceStatus = "Rejected"; }
                    }
                    context.Entry(oldobj).CurrentValues.SetValues(ticket); //change the old ticket to the new ticket
                    context.SaveChanges(); // save to database

                    //add the event in the notification
                    context.Notification.Add(new Notification
                    {
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

        public ActionResult CompletedTicketsSearch(string search)
        {
            using (var context = new ReimbursementContext())
            {
                int userId = (int)Session["UserId"]; //get the user id from session

                //select all tickets that are complete
                var query = context.Ticket.Include(req => req.User)
                     .AsEnumerable().Where(s => s.DateCompleted != null).Where(s => s.CRF.ToString().Contains(search) || s.User.FirstName.ToString().Contains(search) || s.User.LastName.ToString().Contains(search) || s.DateFiled.ToString("MM/dd/yyyy").Contains(search)).ToList();

                ViewBag.pendingTickets = query;
                return View("CompletedTickets");
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