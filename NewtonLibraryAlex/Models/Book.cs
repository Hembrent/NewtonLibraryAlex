using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewtonLibraryAlex.Models
{
    internal class Book
    {
        public int BookID { get; set; }

        [Required]
        [MaxLength(20)]
        public string Title { get; set; }

        // I had thoughts about adding an attribute to keep scoring within a range, but then we can't have humorous scores like -5.0 or 100.0.
        public float Rating { get; set; }
        public DateTime PublicationDate { get; set; }

        public Guid ISBN { get; set; } = Guid.NewGuid();

        public LoanDetails? LoanDetails { get; set; }

        // The connectors to the Authors, both through the connecting class of BookAuthors, and individually to Authors. That way, you can look at the connection and look at them individually
        public ICollection<Author> Authors { get; set; } = new List<Author>();

        //public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

        // Code connecting the loaning of books to the customers that loans them.
        //public int? LoanedToCustomerID { get; set; }
        //public Customer? LoanedToCustomer { get; set; }

        public bool IsLoaned { get; set; }
    }
}
