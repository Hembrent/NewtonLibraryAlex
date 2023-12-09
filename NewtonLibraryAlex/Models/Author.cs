using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewtonLibraryAlex.Models
{
    internal class Author
    {
        public int AuthorID { get; set; }

        // Using attributes, I can force actual inputs and deny empty inputs.
        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(20)]
        public string LastName { get; set; }

        // Connecting the books to the authors, individually and through the connecting class. I like the capacity to see them individually and seeing their connector.
        public ICollection<Book> Books { get; set; } = new List<Book>();

        //public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    }
}
