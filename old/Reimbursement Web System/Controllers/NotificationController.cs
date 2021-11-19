using Reimbursement_Web_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reimbursement_Web_System.Controllers
{
    [SessionAuthorize]
    public class NotificationController : Controller
    {
        public JsonResult GetToasterNotification()
        {
            int userId = (int)Session["UserId"]; //get the user id
            using (var context = new ReimbursementContext())
            {
                // select all notification for the user
                var notification = context.Notification
                               .Where(s => s.UserId == userId)
                               .ToList();

                if (notification.Count() > 0)
                {
                    //delete the notification in the database
                    context.Notification.RemoveRange(notification);
                    context.SaveChanges();

                    //return the notification message in javascript
                    return Json(notification.Select(s => s.message).ToList(), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}