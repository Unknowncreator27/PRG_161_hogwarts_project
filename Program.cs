using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PRG_161_hogwarts_project
{
    // Enum for menu navigation
    public enum MenuOption
    {
        AddNewCustomer = 1,
        AddNewBook,
        RentBook,
        Checkout,
        ShowUsers,
        ShowTransactions,
        CalculateRegistrationDiscount,
        AssignRentalBasedCoupon,
        ShowCoupons,
        Exit
    }

    public class Customer
    {
        public string CustomerID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public double TotalPurchases { get; set; }
        public int LoyaltyPoints { get; set; }
        public bool IsRoomOfRequirementsMember { get; set; }
        public DateTime RegistrationDate { get; set; }
        public int RentalCount { get; set; }
        public DateTime? LastCouponAwarded { get; set; }
        public List<string> Cart { get; set; } // Tracks BookIDs for current rentals
    }

    public class Book
    {
        public string BookID { get; set; }
        public string Title { get; set; }
        public double RentalPrice { get; set; }
    }

    public class Transaction
    {
        public string TransactionID { get; set; }
        public string CustomerID { get; set; }
        public List<string> BookIDs { get; set; } // List of rented BookIDs
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public bool IsEligibleForCoupons { get; set; }
    }

    public class Coupon
    {
        public string CouponID { get; set; }
        public double DiscountPercentage { get; set; }
        public double MinPurchaseAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int FreeRentals { get; set; }
    }

    public class MagicalBook
    {
        public string MagicalBookID { get; set; }
        public string Title { get; set; }
    }

    public class Program
    {
        private static Dictionary<string, Customer> Customers = new Dictionary<string, Customer>();
        private static Dictionary<string, Book> Books = new Dictionary<string, Book>();
        private static Dictionary<string, Transaction> Transactions = new Dictionary<string, Transaction>();
        private static Dictionary<string, Coupon> Coupons = new Dictionary<string, Coupon>();
        private static Dictionary<string, MagicalBook> MagicalBooks = new Dictionary<string, MagicalBook>();
        private static int customerCounter = 1;
        private static int bookCounter = 1;
        private static int transactionCounter = 1;
        private static int couponCounter = 1;
        private static int magicalBookCounter = 1;

        // Adds a new customer to the Customers dictionary
        // Contributed by: [Litha] 
        public static string AddNewCustomer(string name, string email)
        {
            try
            {
                // null checks
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("Name or Email cannot be null or empty");
                    throw new ArgumentNullException("Name or Email cannot be null or empty");
                }

                string emailPattern = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
                if (!Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase))
                {
                    Console.WriteLine("Invalid email format.");
                    throw new ArgumentException("Invalid email format.");
                }

                if (Customers.Values.Any(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("A customer with this email already exists.");
                    throw new InvalidOperationException("A customer with this email already exists.");
                }

                string customerID = "C" + customerCounter.ToString("D3");
                if (Customers.ContainsKey(customerID))
                {
                    Console.WriteLine("CustomerID already exists.");
                    throw new InvalidOperationException("CustomerID already exists.");
                }

                // Creates a new customer
                Customer customer = new Customer
                {
                    CustomerID = customerID,
                    Name = name,
                    Email = email,
                    TotalPurchases = 0.0,
                    LoyaltyPoints = 0,
                    IsRoomOfRequirementsMember = false,
                    RegistrationDate = DateTime.Now,
                    RentalCount = 0,
                    LastCouponAwarded = null,
                    Cart = new List<string>() // Initialize cart
                };
                Customers.Add(customerID, customer);
                customerCounter++;
                Console.WriteLine($"{customerID}: New customer added successfully.");
                DisplayCustomers();
                return $"{customerID}: New customer added successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        // Adds a new book to the Books dictionary
        // Contributed by: [Ozuko]
        public static string AddNewBook(string title, double rentalPrice)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    Console.WriteLine("Title cannot be null or empty");
                    throw new ArgumentNullException("Title cannot be null or empty");
                }

                if (rentalPrice <= 0)
                {
                    Console.WriteLine("Rental price must be positive.");
                    throw new ArgumentException("Rental price must be positive.");
                }

                string bookID = "B" + bookCounter.ToString("D3");
                if (Books.ContainsKey(bookID))
                {
                    Console.WriteLine("BookID already exists.");
                    throw new InvalidOperationException("BookID already exists.");
                }

                Book book = new Book
                {
                    BookID = bookID,
                    Title = title,
                    RentalPrice = rentalPrice
                };
                // Adds the book
                Books.Add(bookID, book);
                bookCounter++;
                Console.WriteLine($"{bookID}: New book added successfully: {title}");
                DisplayBooks();
                return $"{bookID}: New book added successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        // Adds a book to a customer's cart
        // Contributed by: [Litha]
        public static string RentBook(string customerID, string bookID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerID))
                {
                    throw new ArgumentNullException("CustomerID cannot be empty or null.");
                }

                if (!Customers.ContainsKey(customerID))
                {
                    throw new ArgumentException("Customer not found.");
                }
                

                if (string.IsNullOrWhiteSpace(bookID))
                {
                    throw new ArgumentNullException("BookID cannot be empty or null.");
                }

                if (!Books.ContainsKey(bookID))
                {
                    throw new ArgumentException("Book not found.");
                }

                Customers[customerID].Cart.Add(bookID);
                Console.WriteLine($"Book {bookID} added to cart for customer {customerID}.");
                // Display whatever is in the cart
                DisplayCart(customerID);
                return $"Book {bookID} added to cart successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        // Processes checkout, applies discounts, awards coupons, and resets cart
        // Contributed by: [Hanro]
        public static string Checkout(string customerID)
        {
            try
            {
                // Null checks (ensures that the customerID is not null.)
                if (string.IsNullOrWhiteSpace(customerID))
                {
                    throw new ArgumentNullException("CustomerID cannot be empty or null.");
                }

                if (!Customers.ContainsKey(customerID))
                {
                    throw new ArgumentException("Customer not found.");
                }

                Customer customer = Customers[customerID];
                if (customer.Cart.Count == 0)
                {
                    return "No books in cart to checkout.";
                }

                // Calculate total cost
                double totalCost = 0;
                foreach (string bookID in customer.Cart)
                {
                    totalCost += Books[bookID].RentalPrice;
                }

                // Record transaction
                string transactionResult = RecordTransaction(customerID, totalCost, customer.Cart);
                if (transactionResult.StartsWith("Error"))
                {
                    return transactionResult;
                }

                // Apply registration discount
                string discountResult = CalculateRegistrationDiscount(customerID, totalCost);
                double discount = 0;
                if (!discountResult.StartsWith("Error"))
                {
                    discount = double.Parse(discountResult.Replace("Registration discount: $", ""));
                }

                // Award magical bonus (free book for Room of Requirement members)
                string magicalBonus = "No magical bonus awarded.";
                List<string> awardedMagicalBooks = new List<string>();

                if (customer.IsRoomOfRequirementsMember)
                {
                    string magicalBookID = "MB" + magicalBookCounter.ToString("D3");
                    MagicalBook magicalBook = new MagicalBook
                    {
                        MagicalBookID = magicalBookID,
                        Title = "Magical Book: Secrets of the Room"
                    };
                    MagicalBooks.Add(magicalBookID, magicalBook);
                    magicalBookCounter++;
                    awardedMagicalBooks.Add(magicalBookID);
                    magicalBonus = $"Magical Book Awarded: {magicalBook.Title} ({magicalBookID})";
                }
                Console.WriteLine(magicalBonus);

                // Display checkout details
                Console.WriteLine("\nCheckout Summary:");
                Console.WriteLine("-----------------");
                Console.WriteLine($"Customer: {customer.Name} ({customerID})");
                Console.WriteLine("Rented Books:");
                foreach (string bookID in customer.Cart)
                {
                    Console.WriteLine($"  - {Books[bookID].Title} (${Books[bookID].RentalPrice:F2})");
                }
                Console.WriteLine($"Total Cost: ${totalCost:F2}");
                Console.WriteLine($"Registration Discount: ${discount:F2}");
                Console.WriteLine($"Final Cost: ${(totalCost - discount):F2}");
                Console.WriteLine(magicalBonus);

                // Reset cart
                customer.Cart = new List<string>();
                Console.WriteLine("Cart reset.");
                DisplayCustomers();
                return "Checkout completed successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        // Records a transaction for multiple books
        // Contributed by: [Hanro]
        public static string RecordTransaction(string customerID, double amount, List<string> bookIDs)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerID))
                {
                    throw new ArgumentNullException("CustomerID cannot be empty or null.");
                }

                if (!Customers.ContainsKey(customerID))
                {
                    throw new ArgumentException("Customer not found.");
                }

                if (amount <= 0)
                {
                    throw new ArgumentException("Amount must be positive.");
                }

                if (amount > 100000)
                {
                    throw new ArgumentException("Amount exceeds maximum limit.");
                }

                string transactionID = "TID" + transactionCounter.ToString("D3");
                if (Transactions.ContainsKey(transactionID))
                {
                    throw new InvalidOperationException("TransactionID already exists.");
                }

                Transaction transaction = new Transaction
                {
                    TransactionID = transactionID,
                    CustomerID = customerID,
                    BookIDs = new List<string>(bookIDs), // Copy book IDs
                    Amount = amount,
                    Date = DateTime.Now,
                    IsEligibleForCoupons = amount > 50
                };
                Transactions.Add(transactionID, transaction);
                transactionCounter++;

                Customer customer = Customers[customerID];
                customer.TotalPurchases += amount;
                customer.LoyaltyPoints += (int)Math.Floor(amount / 10);
                customer.RentalCount += bookIDs.Count; // Increment by number of books rented

                if (customer.LoyaltyPoints >= 100 && !customer.IsRoomOfRequirementsMember)
                {
                    customer.IsRoomOfRequirementsMember = true;
                    Console.WriteLine("Congratulations! You are now a Room of Requirement member.");
                }

                string couponResult = AssignRentalBasedCoupons(customerID);
                if (couponResult.Contains("Coupon awarded"))
                {
                    Console.WriteLine(couponResult);
                }

                Console.WriteLine($"Recorded transaction: ID={transactionID}, Amount=${amount:F2}");
                DisplayCustomers();
                return $"Transaction recorded: {transactionID}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to record transaction: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        // Awards rental-based coupons based on RentalCount
        // Contributed by: [Hanro]
        public static string AssignRentalBasedCoupons(string customerID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerID))
                {
                    throw new ArgumentNullException("CustomerID cannot be empty or null.");
                }

                if (!Customers.ContainsKey(customerID))
                {
                    throw new ArgumentException("Customer not found.");
                }

                Customer customer = Customers[customerID];
                if (customer.LastCouponAwarded.HasValue && (DateTime.Now - customer.LastCouponAwarded.Value).TotalDays < 30)
                {
                    return "No coupon assigned: One coupon per month limit.";
                }

                int rentalCount = customer.RentalCount;
                int freeRentals = 0;

                if (rentalCount >= 75)
                {
                    freeRentals = 8;
                }
                else if (rentalCount >= 50)
                {
                    freeRentals = 4;
                }
                else if (rentalCount >= 25)
                {
                    freeRentals = 2;
                }
                else if (rentalCount >= 10)
                {
                    freeRentals = 1;
                }

                if (freeRentals > 0)
                {
                    string couponID = "CPID" + couponCounter.ToString("D3");
                    if (Coupons.ContainsKey(couponID))
                    {
                        throw new InvalidOperationException("CouponID already exists.");
                    }

                    Coupon coupon = new Coupon
                    {
                        CouponID = couponID,
                        DiscountPercentage = 0,
                        MinPurchaseAmount = 0,
                        ExpiryDate = DateTime.Now.AddDays(30),
                        FreeRentals = freeRentals
                    };
                    Coupons.Add(couponID, coupon);
                    couponCounter++;
                    customer.LastCouponAwarded = DateTime.Now;
                    Console.WriteLine($"Coupon awarded: {freeRentals} free rental(s).");
                    DisplayCustomers();
                    return $"Coupon awarded: {freeRentals} free rental(s).";
                }

                return "No coupon awarded: Insufficient rentals.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        // Calculates discount based on registration years
        // Contributed by: [Hanro]
        public static string CalculateRegistrationDiscount(string customerID, double purchaseAmount)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerID))
                {
                    throw new ArgumentNullException("CustomerID cannot be empty or null.");
                }

                if (!Customers.ContainsKey(customerID))
                {
                    throw new ArgumentException("Customer not found.");
                }

                if (purchaseAmount <= 0)
                {
                    throw new ArgumentException("Purchase amount must be positive.");
                }

                if (purchaseAmount > 100000)
                {
                    throw new ArgumentException("Purchase amount exceeds maximum limit.");
                }

                double registrationYears = (DateTime.Now - Customers[customerID].RegistrationDate).TotalDays / 365;
                double discountPercentage = 0;

                if (registrationYears >= 15)
                {
                    discountPercentage = 35;
                }
                else if (registrationYears >= 10)
                {
                    discountPercentage = 20;
                }
                else if (registrationYears >= 5)
                {
                    discountPercentage = 10;
                }
                else
                {
                    discountPercentage = 5;
                }

                double discount = purchaseAmount * (discountPercentage / 100);
                return $"Registration discount: ${Math.Round(discount, 2):F2}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }

        // Displays all customers
        // Contributed by: [Ozuko]
        public static void DisplayCustomers()
        {
            if (Customers.Count == 0)
            {
                Console.WriteLine("No customers found.");
                return;
            }
            Console.WriteLine("\nCurrent Customers in the system:");
            Console.WriteLine("-----------------------------------");
            foreach (var customer in Customers.Values)
            {
                Console.WriteLine($"CustomerID: {customer.CustomerID}");
                Console.WriteLine($"Customer Name: {customer.Name}");
                Console.WriteLine($"Customer Email: {customer.Email}");
                Console.WriteLine($"Total Purchases: ${customer.TotalPurchases:F2}");
                Console.WriteLine($"Loyalty Points: {customer.LoyaltyPoints}");
                Console.WriteLine($"IsRoomOfRequirementsMember: {customer.IsRoomOfRequirementsMember}");
                Console.WriteLine($"Date Of Registration: {customer.RegistrationDate:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"Rental Count: {customer.RentalCount}");
                Console.WriteLine($"Last Coupon Awarded: {(customer.LastCouponAwarded.HasValue ? customer.LastCouponAwarded.Value.ToString("yyyy-MM-dd") : "None")}");
                Console.WriteLine($"Cart: {(customer.Cart.Count > 0 ? string.Join(", ", customer.Cart) : "Empty")}");
                Console.WriteLine("-----------------------------------");
            }
        }

        // Displays all books
        // Contributed by: [Litha]
        public static void DisplayBooks()
        {
            if (Books.Count == 0)
            {
                Console.WriteLine("No books found.");
                return;
            }
            Console.WriteLine("\nCurrent Books in the system:");
            Console.WriteLine("-----------------------------------");
            foreach (var book in Books.Values)
            {
                Console.WriteLine($"BookID: {book.BookID}");
                Console.WriteLine($"Title: {book.Title}");
                Console.WriteLine($"Rental Price: ${book.RentalPrice:F2}");
                Console.WriteLine("-----------------------------------");
            }
        }

        // Displays a customer's cart
        // Contributed by: [Hanro]
        public static void DisplayCart(string customerID)
        {
            if (!Customers.ContainsKey(customerID))
            {
                Console.WriteLine("Customer not found.");
                return;
            }
            var cart = Customers[customerID].Cart;
            if (cart.Count == 0)
            {
                Console.WriteLine($"Cart for {customerID} is empty.");
                return;
            }
            Console.WriteLine($"\nCart for {customerID}:");
            Console.WriteLine("-----------------------------------");
            foreach (string bookID in cart)
            {
                if (Books.ContainsKey(bookID))
                {
                    Console.WriteLine($"  - {Books[bookID].Title} (${Books[bookID].RentalPrice:F2})");
                }
            }
            Console.WriteLine("-----------------------------------");
        }

        // Displays all coupons
        // Contributed by: [Hanro]
        public static void DisplayCoupons()
        {
            if (Coupons.Count == 0)
            {
                Console.WriteLine("No coupons found.");
                return;
            }
            Console.WriteLine("\nCurrent Coupons in the system:");
            Console.WriteLine("-----------------------------------");
            foreach (var coupon in Coupons.Values)
            {
                Console.WriteLine($"CouponID: {coupon.CouponID}");
                Console.WriteLine($"Discount Percentage: {coupon.DiscountPercentage}%");
                Console.WriteLine($"Minimum Purchase Amount: ${coupon.MinPurchaseAmount:F2}");
                Console.WriteLine($"Expiry Date: {coupon.ExpiryDate:yyyy-MM-dd}");
                Console.WriteLine($"Free Rentals: {coupon.FreeRentals}");
                Console.WriteLine("-----------------------------------");
            }
        }

        public static void DisplayTransaction()
        {
            if (Transactions.Count == 0)
            {
                Console.WriteLine("No transactions to display.");
                return;
            }
            Console.WriteLine("\nCurrent transactions:");
            Console.WriteLine("-------------------------------------------------------------");
            
            foreach (var transaction in Transactions.Values)
            {
                Console.WriteLine($"Transaction ID: {transaction.TransactionID}");
                
                Console.WriteLine($"Customer ID: {transaction.CustomerID}");
                Console.WriteLine($"Amount: ${transaction.Amount:F2}");
                Console.WriteLine($"Date: {transaction.Date:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"Eligible For Coupon: {transaction.IsEligibleForCoupons}");
                Console.WriteLine("-------------------------------------------------------------");
            }
        }

        
        // Main program loop
        // Contributed by: [Hanro, Litha, Ozuko]
        static void Main(string[] args)
        {

            
            

            while (true)
            {

                int count = 1;
                Console.WriteLine("Choose a number from the list below: ");
                



                try
                {


                    foreach (string item in Enum.GetNames(typeof(MenuOption)))
                    {
                        Console.WriteLine("{0}. {1}", count, item); // Displaying menu options
                        count++; // Incrementing count for next option
                    }

                    int option = int.Parse(Console.ReadLine()); // User input for menu option


                    switch (option)
                    {
                        case 1:
                            Console.WriteLine("Enter customer name: ");
                            string custName = Console.ReadLine();
                            Console.WriteLine("Enter customer email: ");
                            string custEmail = Console.ReadLine();
                            string addResult = AddNewCustomer(custName, custEmail);
                            Console.WriteLine(addResult);
                            break;

                        case 2:
                            Console.WriteLine("Enter book title: ");
                            string title = Console.ReadLine();
                            Console.WriteLine("Enter rental price: ");
                            if (!double.TryParse(Console.ReadLine(), out double rentalPrice))
                            {
                                throw new ArgumentException("Invalid rental price format.");
                            }
                            string bookResult = AddNewBook(title, rentalPrice);
                            Console.WriteLine(bookResult);
                            break;

                        case 3:
                            Console.WriteLine("Enter customerID: ");
                            string rentCustomerID = Console.ReadLine();
                            Console.WriteLine("Books Available: \n");
                            DisplayBooks();
                            Console.WriteLine("\n");
                            Console.WriteLine("Enter bookID: ");
                            string bookID = Console.ReadLine();
                            string rentResult = RentBook(rentCustomerID, bookID);
                            Console.WriteLine(rentResult);
                            break;

                        case 4:
                            Console.WriteLine("Enter customerID: ");
                            string checkoutCustomerID = Console.ReadLine();
                            string checkoutResult = Checkout(checkoutCustomerID);
                            Console.WriteLine(checkoutResult);
                            break;

                        case 5:
                            Console.WriteLine("Showing current users\n");
                            DisplayCustomers();
                            break;

                        case 6:
                            Console.WriteLine("Showing all transactions\n");
                            DisplayTransaction();
                            break;

                        case 7:
                            Console.WriteLine("Enter customerID: ");
                            string calcCustomerID = Console.ReadLine();
                            Console.WriteLine("Enter purchase amount: ");
                            if (!double.TryParse(Console.ReadLine(), out double purchaseAmount))
                            {
                                throw new ArgumentException("Invalid amount format.");
                            }
                            string discountResult = CalculateRegistrationDiscount(calcCustomerID, purchaseAmount);
                            Console.WriteLine(discountResult);
                            DisplayCustomers();
                            break;

                        case 8:
                            Console.WriteLine("Enter customerID: ");
                            string couponCustomerID = Console.ReadLine();
                            string couponResult = AssignRentalBasedCoupons(couponCustomerID);
                            Console.WriteLine(couponResult);
                            if (!couponResult.StartsWith("Error") && !couponResult.Contains("No coupon assigned"))
                            {
                                DisplayCoupons();
                            }
                            break;

                        case 9:
                            Console.WriteLine("Showing all coupons\n");
                            DisplayCoupons();
                            break;

                        case 10:
                            Console.WriteLine("Exiting program...");
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Invalid Menu Option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}