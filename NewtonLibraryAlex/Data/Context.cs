using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NewtonLibraryAlex.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewtonLibraryAlex.Data
{
    internal class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost; Database=NewtonLibraryAlex; Trusted_Connection=True; Trust Server Certificate=Yes; User Id=NewtonLibrary; password=NewtonLibrary");
            // Server=tcp:newtonlibraryalex.database.windows.net,1433;Initial Catalog=NewtonLibraryAlex;Persist Security Info=False;User ID=NewtonLibraryAlex;Password=NewtonLibrary123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Customer> Customers { get; set; }
        //public DbSet<ISBN> ISBNs { get; set; }
        public DbSet<LoanDetails> LoanDetails { get; set; }
        //public DbSet<BookAuthor> BookAuthors { get; set; }
    }
}
