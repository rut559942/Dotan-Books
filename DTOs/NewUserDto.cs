using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace DTOs
    {
        public record NewUserDto
        {
            [Required(ErrorMessage = "Name is required")]
            [StringLength(50, MinimumLength = 2)]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email format")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Phone is required")]
            [Phone]
            public string Phone { get; set; } = string.Empty;
        }
    }


