using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewtonLibraryAlex.Models
{

    internal class LoanDetails
    {
        public int LoanDetailsID { get; set; } // The Primary Key. Given its name followed by ID.
        public int BookID { get; set; } // Foreign Key for the Book class.
        public int CustomerID { get; set; } // Foreign Key for the Customer class.
        public DateTime DateLoaned { get; set; }
        public DateTime? DateReturned { get; set; }

        // Connecting the loaning details between books and customers, and the LoanDetails class itself.
        // Making sure the relevant details are linked in the database.
        public ICollection<Book> Books { get; set; } = new List<Book>(); // The reference to the loaned book.
        public Customer? Customer { get; set; } // The reference to the loaning customer.
    }
}
