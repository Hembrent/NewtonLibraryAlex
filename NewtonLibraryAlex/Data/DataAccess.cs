using Microsoft.EntityFrameworkCore;
using NewtonLibraryAlex.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NewtonLibraryAlex.Data
{
    internal class DataAccess
    {
        public void Seed()
        {
            Context context = new Context();

            var author1 = new Author { FirstName = "Gormley", LastName = "Blomp" };
            var author2 = new Author { FirstName = "Spim", LastName = "Fwob" };
            var author3 = new Author { FirstName = "Plemps", LastName = "Bumble" };

            var customer1 = new Customer { FirstName = "Bob", LastName = "Kelso", CardNumber = "LC" + new Random().Next(100000, 999999).ToString(), CardPin = new Random().Next(1000, 9999) };
            var customer2 = new Customer { FirstName = "Wungus", LastName = "Bungus", CardNumber = "LC" + new Random().Next(100000, 999999).ToString(), CardPin = new Random().Next(1000, 9999) };
            var customer3 = new Customer { FirstName = "Dink", LastName = "Donk", CardNumber = "LC" + new Random().Next(100000, 999999).ToString(), CardPin = new Random().Next(1000, 9999) };

            var book1 = new Book
            {
                Title = "Big Book 1",
                PublicationDate = DateTime.Now,
                Rating = 4.5f
            };

            var book2 = new Book
            {
                Title = "Little Book 2",                
                PublicationDate = DateTime.Now,
                Rating = 3.8f
            };

            var book3 = new Book
            {
                Title = "Gleb",
                PublicationDate = DateTime.Now,
                Rating = 5.0f
            };

            var book4 = new Book
            {
                Title = "AAAAAAAAAAAA",
                PublicationDate = DateTime.Now,
                Rating = 2.0f
            };

            context.Customers.AddRange(customer1, customer2, customer3);
            context.Authors.AddRange(author1, author2, author3);

            // This code links authors to books.
            book1.Authors.Add(author1);
            book1.Authors.Add(author2);
            
            book2.Authors.Add(author1);

            book3.Authors.Add(author2);

            book4.Authors.Add(author3);
            book4.Authors.Add(author1);

            context.Books.AddRange(book1, book2, book3, book4);

            context.SaveChanges();

            Console.WriteLine("The seeder has run its course, and you got a load of junk added.\n");
        }

        public void AddNewBook(string title, DateTime publicationDate, float rating, List<string> authorNames)
        {
            using (var context = new Context())
            {
                // Starting with creating a brand new book.
                var newBook = new Book
                {
                    Title = title,
                    PublicationDate = publicationDate,
                    IsLoaned = false,
                    Rating = rating
                };

                // Then we connect an (or several) author(s) to the created book.
                foreach (var authorName in authorNames)
                {
                    // Trim the author name to handle spaces outside of the MIDDLE, where the space belongs.
                    var trimAuthorName = authorName.Trim();

                    // Check if the author name follows the expected format.
                    if (!trimAuthorName.Contains(" "))
                    {
                        Console.WriteLine($"You did it wrong: {trimAuthorName}. You're supposed to do it like this: 'Firstname Lastname'.\n");
                        continue;
                    }

                    // Check if the author exists, otherwise creating a brand new author on the spot.
                    var author = context.Authors.FirstOrDefault(a => a.FirstName + " " + a.LastName == trimAuthorName);

                    if (author == null)
                    {
                        // Now, if the author does not exist when making the book, it's gonna take the input and do a little Splitting.
                        var names = trimAuthorName.Split();

                        // So here it creates the new author directly, splitting their name according to
                        // how their name structure should be.
                        author = new Author { FirstName = names[0], LastName = names[1] };

                        // And then we slap that new author into the context!
                        context.Authors.Add(author);

                        Console.WriteLine($"{author.FirstName} {author.LastName} added for you, because they weren't registered before!\n");
                    }

                    // And here we connect the book and author.
                    newBook.Authors.Add(author);
                }

                // Finally, jam that book into the context so it exists.
                context.Books.Add(newBook);

                context.SaveChanges();

                Console.WriteLine($"{newBook.Title} slotted in the shelves!\n");
            }
        }

        public void AddNewAuthor(string firstName, string lastName)
        {
            var context = new Context();

            var newAuthor = new Author { FirstName = firstName, LastName = lastName };

            // Adds the new author to the context, or it won't exist.
            context.Authors.Add(newAuthor);

            context.SaveChanges();

            Console.WriteLine($"{newAuthor.FirstName} {newAuthor.LastName} added!\n");
        }

        public void AddNewCustomer(string firstName, string lastName)
        {
            var context = new Context();

            var newCustomer = new Customer { FirstName = firstName, LastName = lastName };

            // Check model state before saving, I'll leave this code in here as an example from GPT to how I could at least debug
            // the non-functional [Required] attributes. So it appears that in EF7, you need to add some form of validation code.

            //if (!Validator.TryValidateObject(newCustomer, new ValidationContext(newCustomer), null, validateAllProperties: true))
            //{
            //    // Handle validation errors
            //    Console.WriteLine("Validation failed. Unable to save customer.");
            //    return;
            //}

            // Adds the new customer to the context, else it won't actually exist.
            context.Customers.Add(newCustomer);

            context.SaveChanges();

            Console.WriteLine($"{newCustomer.FirstName} {newCustomer.LastName} recruited for the cause!\n");
        }

        public void LoanBookToCustomer(int bookId, int customerId)
        {
            using (var context = new Context())
            {
                // Fetch the book and customer, the one getting loaned and the loaner itself.
                var bookLoan = context.Books.Find(bookId);
                var customerLoan = context.Customers.Find(customerId);

                // Making sure they're found, that they actually exist.
                if (bookLoan != null && customerLoan != null)
                {
                    // This is the latest checker to see if the book is loaned out. As it turns out,
                    // it won't actually hit the bottom else.
                    // Therefore, I have decided to put it up here first. Now it works.
                    if (bookLoan.IsLoaned)
                    {
                        Console.WriteLine("It's already loaned out, clerical error much?\n");
                    }
                    else
                    {
                        // Check that the book itself hasn't already been loaned out first. Hard to loan what's already loaned.
                        if (bookLoan.LoanDetails == null || bookLoan.LoanDetails.DateReturned != null)
                        {
                            // Instantiates the LoanDetails object that's gonna keep track of the book,
                            // the customer, the time it's been loaned and making sure the return is null.
                            var loanDetails = new LoanDetails
                            {
                                BookID = bookLoan.BookID,
                                CustomerID = customerLoan.LibraryCardID,
                                DateLoaned = DateTime.Now,
                                DateReturned = null
                            };

                            // Updating the info to tell the database that it's now loaned.
                            bookLoan.LoanDetails = loanDetails;
                            bookLoan.IsLoaned = true;

                            // Putting the updated LoanDetails into the context.
                            context.LoanDetails.Add(loanDetails);

                            // Updating the Loans property in the Customer class, the one that's an ICollection of LoanDetails.
                            customerLoan.Loans.Add(loanDetails);

                            context.SaveChanges();

                            Console.WriteLine($"'{bookLoan.Title}' has been checked out to {customerLoan.FirstName} {customerLoan.LastName}. Bill them if they run late. How much is late? Don't care, bill them.\n");
                        }
                        else
                        {
                            // And if the book is already loaned out, though I noticed that it wasn't actually being used,
                            // so this is now a legacy function. I leave the code as-is so you can see how I originally designed it.
                            Console.WriteLine("It's already loaned out, clerical error much?");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("You know you have to create a book and a customer before calling their IDs, right? Sheesh.\n");
                }
            }
        }

        public void ReturnBook(int bookId, int customerId)
        {
            using (var context = new Context())
            {
                // Here we fetch the relevant book and customer from the database, in order to return the book.
                var bookReturning = context.Books.Find(bookId);
                var customerReturning = context.Customers.Find(customerId);

                // Then we check if they can even be found in the first place, notice how it checks that the two Finds aren't fetching null values.
                if (bookReturning != null && customerReturning != null)
                {
                    // This piece of garbage became a necessity to *force* a retrieval of data, by means of explicit loading.
                    // You would not believe how much of an obnoxious bother this method was without this.
                    context.Entry(customerReturning).Collection(c => c.Loans).Load();

                    // The checker to see if the book is loaned in the first place, and also not already returned.
                    // Redundant, but it was a theory that having a null space at all might've thrown the else instead of the if.
                    // That wasn't the case.
                    var loanDetails = customerReturning.Loans.FirstOrDefault(loan => loan.BookID == bookId && loan.DateReturned == null);

                    if (loanDetails != null)
                    {
                        // Updating the new loaning details, sending the book back into the library for loaning to someone else.
                        loanDetails.DateReturned = DateTime.Now;
                        bookReturning.IsLoaned = false;
                        context.SaveChanges();
                        // Detach the customerReturning entity from the context
                        context.Entry(customerReturning).State = EntityState.Detached;

                        Console.WriteLine($"Book '{bookReturning.Title}' was returned by {customerReturning.FirstName} {customerReturning.LastName}. Bill them another hundo as a late fee.\n");
                    }
                    else
                    {
                        // The else that pops up if the book isn't actually loaned yet, or if it's already been returned.
                        // Or if you've mixed up who's loaned what!
                        Console.WriteLine("The book's not loaned, yo. Or you've mixed up who's loaning it. Check your numbers.\n");
                    }
                }
                else
                {
                    // And if you've forgotten to add either book or customer, this executes.
                    // Maybe you should check yourself before you Shrek yourself?
                    Console.WriteLine("You know you have to add these details before actually trying to return something. They don't exist, man!\n");
                }
            }
        }

        public void DeleteCustomer(int customerId)
        {
            using (var context = new Context())
            {
                var customerDelete = context.Customers.Find(customerId);

                if (customerDelete != null)
                {
                    // Explicitly load the Loans collection
                    context.Entry(customerDelete).Collection(c => c.Loans).Load();

                    // Check if the customer is loaning any books
                    if (customerDelete.Loans.Any(loan => loan.DateReturned == null))
                    {
                        Console.WriteLine($"Can't delete '{customerDelete.FirstName} {customerDelete.LastName}', they have one of our books!\n");
                    }
                    else
                    {
                        // Deletes the customer from the context.
                        context.Customers.Remove(customerDelete);
                        context.SaveChanges();
                        Console.WriteLine($"'{customerDelete.FirstName} {customerDelete.LastName}' WAS KILLED! YOU KILLED THEM!\n");
                    }
                }
                else
                {
                    // Just pops up if you're getting ahead of yourself, or maybe typoing the ID.
                    Console.WriteLine("THEY'RE ALREADY DEAD! Or they weren't added yet.\n");
                }
            }
        }


        public void DeleteBook(int bookId)
        {
            using (var context = new Context())
            {
                var bookDelete = context.Books.Find(bookId);

                if (bookDelete != null)
                {
                    // By explicitly loading the information from the database,
                    // we can query the Customer's ID through LoanDetails, which connects Book and Customer.
                    // Load() is used to execute this query.
                    // This might be something worth taking up in future courses,
                    // because it seems like lazy loading is causing many pointless issues.
                    context.Entry(bookDelete)
                        .Reference(b => b.LoanDetails)
                        .Query()
                        .Include(ld => ld.Customer)
                        .Load();

                    // Checking if the book has been loaned out first, can't remove a book that's not even in your hands currently.
                    if (bookDelete.LoanDetails != null && bookDelete.LoanDetails.DateReturned == null)
                    {                        
                        var loanedToCustomer = bookDelete.LoanDetails.Customer;

                        if (loanedToCustomer != null)
                        {
                            Console.WriteLine($"Can't shred '{bookDelete.Title}', SOMEONE has it on loan, that SOMEONE is: {loanedToCustomer.FirstName} {loanedToCustomer.LastName}. Go bother them instead.\n");
                            return;
                        }
                        else
                        {
                            Console.WriteLine($"Can't destroy '{bookDelete.Title}', we don't know who has it, their information's gone. Did you tamper with our records?\n");
                            return;
                        }
                    }

                    // If the book is not on loan or has been returned, get shredding.
                    Console.WriteLine($"Shredding '{bookDelete.Title}'\n");

                    // Deleting the book from the context.
                    context.Books.Remove(bookDelete);
                    context.SaveChanges();

                    Console.WriteLine($"'{bookDelete.Title}' has been erased from this plane of existence. This is your sin.\n");
                }
                else
                {
                    // Book not found
                    Console.WriteLine("Write the book first, or check your typos. Everyone's laughing at you right now.\n");
                }
            }
        }

        public void DeleteAuthor(int authorId)
        {
            using (var context = new Context())
            {
                var authorDelete = context.Authors.Find(authorId);

                if (authorDelete != null)
                {
                    foreach (var book in authorDelete.Books.ToList())
                    {
                        book.Authors.Remove(authorDelete);

                        // This was intended to work the same, but the cascade deletion never triggers,
                        // so I'm once again just leaving it.
                        if (book.Authors.Count == 0)
                        {
                            context.Books.Remove(book);
                        }
                    }

                    context.Authors.Remove(authorDelete);
                    context.SaveChanges();
                    Console.WriteLine($"'{authorDelete.FirstName} {authorDelete.LastName}' is gone with the wind. Erased.\n");
                }
                else
                {
                    // Author no exist.
                    Console.WriteLine("Author's not here, man.\n");
                }
            }
        }

        public void DisplayLoaningDetails()
        {
            using (var context = new Context())
            {
                // Linking together customers and the books so that you can see who's loaned what in the console.
                var loans = context.LoanDetails.Include(ld => ld.Customer).Include(ld => ld.Books).Where(ld => ld.DateReturned == null).ToList();

                if (loans.Any()) // Tidy little thing to look for any details that might exist in LoanDetails.
                {
                    foreach (var loan in loans)
                    {
                        // Explicitly reload loan details from the database
                        context.Entry(loan).Reload();

                        Console.WriteLine("Customer details:");
                        Console.WriteLine($"  - Loan ID: {loan.LoanDetailsID}");
                        Console.WriteLine($"    Customer ID: {loan.CustomerID}");
                        Console.WriteLine($"    Customer: {loan.Customer.FirstName} {loan.Customer.LastName}\n");
                        Console.WriteLine("Book details:");

                        foreach (var loanedBook in loan.Books)
                        {
                            Console.WriteLine($"  - ID: {loanedBook.BookID}");
                            Console.WriteLine($"    Title: {loanedBook.Title}");
                            Console.WriteLine($"    ISBN: {loanedBook.ISBN}");
                        }

                        Console.WriteLine($"    Date Loaned: {loan.DateLoaned}");
                        Console.WriteLine($"    Date Returned: {loan.DateReturned ?? DateTime.Now}");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("All books accounted for, nobody's loaned shit today.\n");
                }
            }
        }

        public void ShowAllLibraryData()
        {
            using (var context = new Context())
            {
                // Linking all the relevant tables together so you can look at all the relevant data in the console.
                var customers = context.Customers.ToList();
                var books = context.Books.Include(b => b.Authors).ToList();
                var authors = context.Authors.ToList();

                if (customers.Any() || books.Any() || authors.Any())
                {
                    Console.WriteLine("All Library Data:");

                    if (customers.Any())
                    {
                        Console.WriteLine("\nCustomers:");
                        foreach (var customer in customers)
                        {
                            Console.WriteLine($"ID: {customer.LibraryCardID}, Name: {customer.FirstName} {customer.LastName}");
                        }
                    }

                    if (books.Any())
                    {
                        Console.WriteLine("\nBooks:");
                        foreach (var book in books)
                        {
                            Console.WriteLine($"ID: {book.BookID}, Title: {book.Title}, Authors: {string.Join(", ", book.Authors.Select(a => a.FirstName + " " + a.LastName))}");
                        }
                    }

                    if (authors.Any())
                    {
                        Console.WriteLine("\nAuthors:");
                        foreach (var author in authors)
                        {
                            Console.WriteLine($"ID: {author.AuthorID}, Name: {author.FirstName} {author.LastName}\n");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No library data found.\n");
                }
            }
        }

        public void DisplayAuthorsAndBooks()
        {
            using (var context = new Context())
            {
                // Linking the Authors and Books in a list so that they can be displayed together.
                var authors = context.Authors.Include(a => a.Books).ToList();

                if (authors.Any())
                {
                    foreach (var author in authors)
                    {
                        Console.WriteLine($"Author ID: {author.AuthorID}");
                        Console.WriteLine($"Author: {author.FirstName} {author.LastName}\n");

                        if (author.Books.Any())
                        {
                            Console.WriteLine("Books:");
                            foreach (var book in author.Books)
                            {
                                Console.WriteLine($"-  Book ID: {book.BookID}");
                                Console.WriteLine($"   Book title: {book.Title}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("They got no books here, don't pay them.\n");
                        }

                        Console.WriteLine(); // Creating a tidy little gap for easier readability.
                    }
                }
                else
                {
                    Console.WriteLine("No authors found.\n");
                }
            }
        }

        public void ResetDatabase()
        {
            using (Context context = new Context())
            {
                // Code to remove all the records from the context and thus the database.
                context.Authors.RemoveRange(context.Authors);
                context.Books.RemoveRange(context.Books);
                context.Customers.RemoveRange(context.Customers);
                context.LoanDetails.RemoveRange(context.LoanDetails);

                // The code that specifically resets each table so that you can test without having to adjust for
                // continually incrementing ID slots.
                var resetAuthorsSql = "DBCC CHECKIDENT ('Authors', RESEED, 0)";
                context.Database.ExecuteSqlRaw(resetAuthorsSql);

                var resetBooksSql = "DBCC CHECKIDENT ('Books', RESEED, 0)";
                context.Database.ExecuteSqlRaw(resetBooksSql);

                var resetCustomersSql = "DBCC CHECKIDENT ('Customers', RESEED, 0)";
                context.Database.ExecuteSqlRaw(resetCustomersSql);

                var resetLoanDetailsSql = "DBCC CHECKIDENT ('LoanDetails', RESEED, 0)";
                context.Database.ExecuteSqlRaw(resetLoanDetailsSql);

                context.SaveChanges();

                Console.WriteLine("All data gone. Now you can work with a fresh slate, clean and serene.");
            }
        }
    }
}

