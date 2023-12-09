using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewtonLibraryAlex.Models
{
    internal class Customer
    {
        [Key]
        public int LibraryCardID { get; set; }

        [Required(ErrorMessage = "FirstName is required.")]
        [RegularExpression(@"\S+", ErrorMessage = "FirstName cannot contain whitespace.")]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        public string LastName { get; set; }

        // Setting the properties of their loan cards, I did once keep the LibraryCardID as a separate property here,
        // but then it clashed with another ID.
        // So now LibraryCardID is the CustomerID in general, it serves the same purpose anyway.
        public string CardNumber { get; set; } = "LC" + new Random().Next(100000, 999999).ToString();
        public int CardPin { get; set; } = new Random().Next(1000, 9999);

        // The connection between customers and books for loaning purposes, via LoanDetails.
        public ICollection<LoanDetails> Loans { get; set; } = new List<LoanDetails>();
    }
}
