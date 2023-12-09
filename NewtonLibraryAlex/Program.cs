using NewtonLibraryAlex.Data;
using NewtonLibraryAlex.Models;
using System.ComponentModel.DataAnnotations;

namespace NewtonLibraryAlex
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataAccess dataAccess = new DataAccess();
            Console.WriteLine("WELCOME TO THE BOOK PIT, LOSER.\n");

            dataAccess.Seed();

            // Creates authors on their own, so you can add them ahead of the books, if you want to.
            // You can copy the codeline to create multiples at the same time. FirstName, LastName order.
            dataAccess.AddNewAuthor("Jingle", "John");

            // Creates customers, much the same as above!
            dataAccess.AddNewCustomer("Namn", "Dum");

            // Creates a list of author names, will utilize authors that already exist and create new ones if the name is
            // not in the database already. Utilizing a Splitter, it will take a first and last name in this format.
            var authorNames = new List<string> { "Moppskaft Skitunge", "Jingle John" };

            // Creates a new book and associate it with the listed authors.
            // Note how it goes "name", DateTime, Float and then the authorNames list.
            dataAccess.AddNewBook("Glorp", DateTime.Now, 3.5f, authorNames);

            // Loaning code, attaches a book's ID to a customer's ID, replace the numbers to account for
            // the book and customer you want respectively. In that order.
            dataAccess.LoanBookToCustomer(1, 1);
            dataAccess.LoanBookToCustomer(2, 2);

            // Same order, but returns the book instead!
            dataAccess.ReturnBook(1, 1);
            dataAccess.ReturnBook(2, 2);

            // Deletion methods, replace the numbers with the IDs of the customers, authors and books you want to delete.
            dataAccess.DeleteCustomer(1);
            dataAccess.DeleteAuthor(1);
            dataAccess.DeleteBook(1);

            // These three do what they say on the tin. With a lot of data, it's gonna get messy.
            dataAccess.DisplayLoaningDetails();
            dataAccess.ShowAllLibraryData();
            dataAccess.DisplayAuthorsAndBooks();

            // USE WITH CARE, THIS RESETS ALL THE DATA INCLUDING IDENTITIES. It's for tidying up after a lot of testing.
            //dataAccess.ResetDatabase();

        }
    }
}