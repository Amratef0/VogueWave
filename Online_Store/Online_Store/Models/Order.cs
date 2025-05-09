using System.ComponentModel.DataAnnotations;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Online_Store.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [RegularExpression(@"^01[0-2,5]{1}[0-9]{8}$", ErrorMessage = "Phone number must be a valid Egyptian number.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 500 characters.")]
        public string? Address { get; set; }

        public DateTime? OrderDate { get; set; }

        public ICollection<OrderProduct>? OrderProducts { get; set; }

        public string? OrderNumber { get; set; }

        public OrderStatus? Status { get; set; } = OrderStatus.Pending;


    }
}