using Reimbursement_Web_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;


namespace Reimbursement_Web_System.Controllers
{
    [SessionAuthorize]
    public class UserController : Controller
    {
        public ActionResult PendingTickets()
        {
            using (var context = new ReimbursementContext())
            {
                int userId = (int)Session["UserId"]; // get the userid from session
                var query = context.Ticket.Where(s => s.User.Id == userId && s.DateCompleted == null).Select(p => p).ToList(); //select all the tickets for the user except completed ticket

                ViewBag.pendingTickets = query; //store the data in view bag for frontend consumption
                return View();
            }
        }
        public ActionResult CompletedTickets()
        {
            using (var context = new ReimbursementContext())
            {
                int userId = (int)Session["UserId"]; // get the userid from session
                var query = context.Ticket.Where(s => s.User.Id == userId && s.DateCompleted != null).Select(p => p).ToList(); //select all the tickets for the user that have date completed value

                ViewBag.pendingTickets = query; //store the data in view bag for frontend consumption
                return View();
            }
        }
        public static string getstatus( string identifiyvalue ,string nameofvalue)
        {
            string approvevalue = "Approved";
            string rejectedvalue = "Rejected";
            if (nameofvalue.Contains("Approved"))
            {
                return approvevalue;
            }
            return rejectedvalue;
        }
        public ActionResult CreateTicket(Ticket ticket)
        {
            ViewBag.PageTitle = "Create Ticket"; //page title
            Session["Reimbursement"] = null; //clear Reimbursement session
            Session["CRF"] = null; //clear CRF session

            ticket = new Ticket(); //create new ticket
            ticket.DateFiled = DateTime.Now; //set the ticket date filed to date today
            ModelState.Clear(); //clear validation
            return View(ticket);
        }

        public ActionResult ViewTicket(int crf)
        {
            ViewBag.PageTitle = "View Ticket"; //page title
            Session["Reimbursement"] = null; //clear Reimbursement session

            using (var context = new ReimbursementContext()) //initialize database connection
            {
                //select the ticket and all necessary data
                var query = context.Ticket
                    .Include(req => req.User)
                    .Include(req => req.Medias)
                    .Where(s => s.CRF == crf)
                    .Select(p => p).SingleOrDefault();

                //select the reimbursement
                var reimbursement = context.Reimbursement
                    .Where(s => s.Ticket.CRF == crf)
                    .OrderBy(s => s.Amount)
                    .Select(p => p).ToList();

                Session["CRF"] = crf; //store crf in session
                Session["Reimbursement"] = reimbursement; //store reimbursement in session
                ViewBag.reimbursements = reimbursement; //store reimbursement in view bag

                return View("CreateTicket", query); //show CreateTicket page
            }
        }

        public ActionResult SaveTicket(Ticket ticket, string command)
        {
            List<Reimbursement> existingReimbursements = Session["Reimbursement"] as List<Reimbursement>;
            if (existingReimbursements == null || existingReimbursements.Count == 0) //validate Reimbursement
            {
                ModelState.AddModelError("", "Please check your Reimbursement"); //add error to model
            }
            else
            {
                var totalAmount = existingReimbursements.Sum(x => x.Amount); 
                if (totalAmount < 500) //validate Reimbursement
                {
                    ModelState.AddModelError("", "Total Amount should be greater than 500"); //add error to model
                }
            }

            ticket.DateFiled = DateTime.Now; //set datefiled to date today

            if (ModelState.IsValid)
            {
                using (var context = new ReimbursementContext()) //initialize dabtase connection
                {
                    if (command.Equals("Create")) //if the ticket is new
                    {
                        int userId = (int)Session["UserId"]; //get user id from session
                        ticket.User = context.User.Where(c => c.Id == userId).FirstOrDefault(); //select user from database

                        List<Media> medias = new List<Media>();
                        if (ticket.ImagesUpload.Count() != 0)
                        {
                            string uploadDir = "Ticket_Images"; //server folder name
                            string fileName;
                            foreach (var rec in ticket.ImagesUpload) //check if there are new images
                            {
                                if (rec != null)
                                {
                                    fileName = Path.GetFileName(rec.FileName); //get the path from the client machine
                                    //generate computed file name
                                    fileName = fileName.Substring(0, fileName.IndexOf('.')) + "_" + DateTime.Now.Millisecond + "-" + DateTime.Now.Second + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Hour + "." + fileName.Substring(fileName.IndexOf('.') + 1);
                                    //save the file in the server
                                    rec.SaveAs(Path.Combine(Server.MapPath("~/" + uploadDir), fileName));
                                    //collect the file path to be save in database
                                    medias.Add(new Media
                                    {
                                        ImagePath = "/" + uploadDir + "/" + fileName,
                                        //UploadDate = DateTime.Now.ToString()
                                    }) ;
                                }
                            }
                        }

                        //set the images to the ticket
                        ticket.Medias = medias;

                        //create the ticket in the database
                        context.Ticket.Add(ticket);

                        //save the database
                        context.SaveChanges();

                        //for each Reimbursements set the ticket id
                        foreach (var rec in existingReimbursements)
                        {
                            rec.Ticket = ticket;
                        }
                        //add the Reimbursements in the Reimbursement table
                        context.Reimbursement.AddRange(existingReimbursements);
                        //save the database
                        context.SaveChanges();
                    }
                    else
                    {
                        //get the existing ticket
                        var dbTicket = context.Ticket
                            .Where(x => x.CRF == ticket.CRF)
                            .SingleOrDefault();
                        //remove all medias 
                        var medias = context.Media
                          .Where(s => s.Ticket.CRF == ticket.CRF)
                          .Select(p => p).ToList();
                        context.Media.RemoveRange(medias);
                        context.SaveChanges();
                        
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
                                    }); 
                                }
                            }
                        }
                        //add the medias in the media model
                        context.Media.AddRange(ticket.Medias);

                        //save the database
                        context.SaveChanges();


                        //update the existing ticket to the new ticket
                        context.Entry(dbTicket).CurrentValues.SetValues(ticket);

                        //save in database
                        context.SaveChanges();

                        //remove all reimbursement 
                        var reimbursement = context.Reimbursement
                          .Where(s => s.Ticket.CRF == ticket.CRF)
                          .Select(p => p).ToList();
                        context.Reimbursement.RemoveRange(reimbursement);
                        context.SaveChanges();

                        foreach (var rec in existingReimbursements)
                        {
                            rec.Ticket = dbTicket; //set the Reimbursement ticker number
                        }

                        //add the tickets in the Reimbursement model
                        context.Reimbursement.AddRange(existingReimbursements);

                        //save the database
                        context.SaveChanges();
                    }
                }
                return RedirectToAction("PendingTickets", "User");
            }
            else
            {
                ViewBag.reimbursements = Session["Reimbursement"] as List<Reimbursement>; //get the Reimbursement from the session
                if (command.Equals("Create"))
                {
                    ViewBag.PageTitle = "Create Ticket"; //set the page title
                }
                else
                {
                    using (var context = new ReimbursementContext()) //initialize the database
                    {
                        //get the existing ticket 
                        var oldTicket = context.Ticket
                            .Include(x => x.Medias)
                            .Where(x => x.CRF == ticket.CRF)
                            .SingleOrDefault();

                        ViewBag.PageTitle = "View Ticket"; //set the page title
                        ticket.Medias = oldTicket.Medias; //get existing media
                    }
                }
                return View("CreateTicket", ticket); //return the ticket view
            }
        }

        public ActionResult DeleteTicket(int crf)
        {
            using (var context = new ReimbursementContext())
            {
                //get all the reimbursement for the ticket
                var reimbursement = context.Reimbursement
                    .Where(s => s.Ticket.CRF == crf)
                    .Select(p => p).ToList();

                //remove all the reimbursement from the model
                context.Reimbursement.RemoveRange(reimbursement);

                //save the database
                context.SaveChanges();

                //the the ticket
                var ticket = context.Ticket
                   .Where(s => s.CRF == crf)
                   .Select(p => p).ToList();

                //remove the ticker from the model
                context.Ticket.RemoveRange(ticket);

                //save the database
                context.SaveChanges();

                //load pending tickets table
                return RedirectToAction("PendingTickets", "User");
            }
        }
        public ActionResult ViewCompletedTicket(int crf)
        {
            ViewBag.PageTitle = "View Completed Ticket"; //set the ticket page title
            Session["Reimbursement"] = null; //clear the session

            using (var context = new ReimbursementContext())
            {
                //load the ticket and necessary data
                var query = context.Ticket
                    .Include(req => req.User)
                    .Include(req => req.Reimbursement)
                    .Include(req => req.Medias)
                    .Where(s => s.CRF == crf)
                    .Select(p => p).SingleOrDefault();

                //load the reimbursement
                var reimbursement = context.Reimbursement
                   .Where(s => s.Ticket.CRF == crf)
                   .OrderBy(s => s.Amount)
                   .Select(p => p).ToList();

                //store the reimbursement in view bag
                ViewBag.reimbursements = reimbursement;

                return View(query);
            }
        }

        public ActionResult ViewReimbursement(Reimbursement reimbursement, int? id, string purpose, string office)
        {
            //view ViewReimbursement modal

            Session["purpose"] = purpose; //save the purpose in session
            Session["office"] = office; //save the office in session

            if (id == null)
            {
                ViewBag.status = "New"; //new Reimbursement
                Session["ReimbursementIndex"] = null; //clear ReimbursementIndex sesion
                reimbursement.Date = DateTime.Now; //set the date of the Reimbursement
            }
            else
            {
                ViewBag.status = "Existing"; //exisitng Reimbursement
                List<Reimbursement> existingReimbursements = Session["Reimbursement"] as List<Reimbursement>; //get the exsitng Reimbursements
                int index = id.Value - 1; //since we used id we - 1 to get the index value
                Session["ReimbursementIndex"] = index; //store the index in session
                reimbursement = existingReimbursements[index]; //get this specific Reimbursement based on the index
            }

            return PartialView(reimbursement); //load the modal
        }

        public ActionResult SaveReimbursement(Reimbursement reimbursement)
        {
            if (ModelState.IsValid) //if there are no errors
            {
                //check if Reimbursement is null if so initialize a new variable
                if (Session["Reimbursement"] == null)
                {
                    Session["Reimbursement"] = new List<Reimbursement>();
                }

                List<Reimbursement> reimbursements = new List<Reimbursement>();
                List<Reimbursement> existingReimbursements = Session["Reimbursement"] as List<Reimbursement>; //get all existingReimbursements from the session

                if (Session["ReimbursementIndex"] == null) //if no ReimbursementIndex is present then this is a new reimbursements
                {
                    reimbursements.Add(reimbursement); //add the reimbursements in the model
                    existingReimbursements.AddRange(reimbursements); //combine the new reimbursement with the existing reimbursements
                }
                else
                {
                    //modify the existing existingReimbursements data
                    existingReimbursements[(int)Session["ReimbursementIndex"]].Date = reimbursement.Date; 
                    existingReimbursements[(int)Session["ReimbursementIndex"]].Amount = reimbursement.Amount;
                    existingReimbursements[(int)Session["ReimbursementIndex"]].NatureOfExpenditure = reimbursement.NatureOfExpenditure;
                }
                //store the existingReimbursements in session
                Session["Reimbursement"] = existingReimbursements;
            }

            //check if the ticket is existing already 
            if (Session["CRF"] == null)
            {
                //check CreateTicketWithReimbursement function
                return RedirectToAction("CreateTicketWithReimbursement");
            }
            else
            {
                //check ViewTicketWithReimbursement function
                return RedirectToAction("ViewTicketWithReimbursement");
            }
        }

        public ActionResult CreateTicketWithReimbursement(Ticket ticket)
        {
            ViewBag.PageTitle = "Create Ticket"; //set the page title
            ViewBag.reimbursements = Session["Reimbursement"] as List<Reimbursement>; //set and get the Reimbursement from session

            ticket.DateFiled = DateTime.Now; //set the date files
            ticket.Purpose = (string)Session["purpose"]; //get the purpose from session
            ticket.Office = (string)Session["office"]; //get the office from session

            ModelState.Clear(); //clear the model
            return View("CreateTicket", ticket); //return create ticket page
        }
        public ActionResult ViewTicketWithReimbursement(Ticket ticket)
        {
            ViewBag.PageTitle = "View Ticket"; //set the page title
            int crf = (int)Session["CRF"]; //get the CRF from the session
            using (var context = new ReimbursementContext())
            {
                //get the ticket info including related data
                var query = context.Ticket
                    .Include(req => req.User)
                    .Include(req => req.Medias)
                    .Where(s => s.CRF == crf)
                    .Select(p => p).SingleOrDefault();

                List<Reimbursement> existingReimbursements = Session["Reimbursement"] as List<Reimbursement>; //get the Reimbursements from the session
                ViewBag.reimbursements = existingReimbursements; //store the Reimbursements in view bag

                ModelState.Clear(); // clear the model
                return View("CreateTicket", query); // load CreateTicket page
            }
        }
        public ActionResult DeleteReimbursement(int id)
        {
            //get all Reimbursement from the table
            List<Reimbursement> existingReimbursements = Session["Reimbursement"] as List<Reimbursement>;
            int index = id - 1;
            //remove the Reimbursement based on the index
            existingReimbursements.RemoveAt(index);

            if (Session["CRF"] == null) //check if the ticket is existing
            {
                return RedirectToAction("CreateTicketWithReimbursement"); //go to CreateTicketWithReimbursement function
            }
            else
            {
                return RedirectToAction("ViewTicketWithReimbursement"); //go to ViewTicketWithReimbursement function
            }
        }
    }
}
