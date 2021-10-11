using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Reimbursement_Web_System.Models
{
    public enum Role
    {
        Standard, Director, HSU, HR, SDAS, Finance
    }

    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "User Name")]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public Role Role { get; set; }
    }
}