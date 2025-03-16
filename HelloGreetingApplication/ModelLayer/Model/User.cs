using System;
using System.ComponentModel.DataAnnotations;

namespace ModelLayer.Model
{
	
    public class User
    {
            [Key]
            public int UserId { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; }
            [Required]
            public string PasswordHash { get; set; }
        }
}

