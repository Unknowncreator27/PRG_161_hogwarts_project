using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PRG_161_hogwarts_project
{

    
    public class  Customer
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

    }

    public class Transaction
    {
        public string TransactionID { get; set; }
        public string customerID { get; set; }
        public double amount { get; set; }
        public DateTime Date { get; set; }
        public bool IsEligibleForCoupons { get; set; }
    }
        internal class Program
        {

            public static void DisplayCustomers()
            {
                if (Customers.Count == 0)
                {
                    Console.WriteLine("No customers found.");
                    return;
                }
                Console.WriteLine("\nCurrent Customers in the system.");
                Console.WriteLine("-----------------------------------");
                foreach (var customer in Customers.Values)
                {
                    Console.WriteLine($"CustomerID: {customer.CustomerID}");
                    Console.WriteLine($"Customer Name: {customer.Name}");
                    Console.WriteLine($"Customer Email: {customer.Email}");
                    Console.WriteLine($"Total Purchases: {customer.TotalPurchases}");
                    Console.WriteLine($"Loayalty points: {customer.LoyaltyPoints}");
                    Console.WriteLine($"IsRoomOfRequirementsMember: {customer.IsRoomOfRequirementsMember}");
                    Console.WriteLine($"Date Of Registration: {customer.RegistrationDate}");
                    Console.WriteLine($"Rental Count: {customer.RentalCount}");
                    Console.WriteLine($"Last Coupon Awarded:" +
                        $" {(customer.LastCouponAwarded.HasValue ? customer.LastCouponAwarded.Value.ToString("yyyy-MM-dd") : "None")}");
                    Console.WriteLine("----------------------------------------------");

                }
            }

            public static void DisplayTransaction()
            {
                if (transactionCounter == 0)
                {
                    Console.WriteLine("No transactions to display.");
                    return;
                } else
                {
                    Console.WriteLine("Current transactions: ");
                    Console.WriteLine("-------------------------------------------------------------");
                    foreach (var transaction in Transactions.Values)
                    {
                        Console.WriteLine($"Transaction ID: {transaction.TransactionID}");
                        Console.WriteLine($"Customer ID: {transaction.customerID}");
                        Console.WriteLine($"Amount: {transaction.amount:F2}");
                        Console.WriteLine($"Date: {transaction.Date}");
                        Console.WriteLine($"Eligible For Coupon: {transaction.IsEligibleForCoupons}");
                        Console.WriteLine("----------------------------------------------------------");
                    }
                }

                
            }

            public static string AddNewCustomer(string name, string email)
            {
                try
                {
                    if (name == null || email == null)
                    {
                        Console.WriteLine("Name or Email cannot be null");
                        throw new ArgumentNullException("Name or Email cannot be null");


                    }

                    string emailPattern = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$";
                    if (!Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase))
                    {
                        Console.WriteLine("Invalid email format.");
                        throw new ArgumentException("Invalid email format.");

                    }
                    //if (Customers.Values.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    //{
                    //    Console.WriteLine("A customer with this email already exists.");
                    //    throw new InvalidOperationException("A customer with this email already exists.");

                    //}

                    // Check for existing customer with the same email (case-insensitive)
                    if (Customers.Values.Any(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                    {
                        Console.WriteLine("A customer with this email already exists.");
                        throw new InvalidOperationException("A customer with this email already exists.");

                    }

                    // create a unique customerID
                    string customerID = "C" + customerCounter.ToString("D3") + 1;
                    Console.WriteLine("CustomerID created: {0}", customerID);

                    if (Customers.ContainsKey(customerID))
                    {
                        Console.WriteLine("CustomerID already exits.");
                        throw new InvalidOperationException("CustomerID already exits.");
                    }

                    // Create the new customer
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
                        LastCouponAwarded = null

                    };
                    Customers.Add(customerID, customer);
                    customerCounter++;
                    Console.WriteLine($"{customerID}: New customer added successfully.");
                    return $"{customerID}: New customer added successfully.";

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return $"Error: {ex.Message}";

                }



            }

            public static string RecordTransaction(string customerID, double amount)
            {
                try
                {
                    if (customerID == null)
                    {
                        throw new ArgumentNullException("CustomerID cannot be null.");

                    }

                    if (!Customers.ContainsKey(customerID))
                    {
                        throw new ArgumentException("Customer not found.");

                    }

                    if (amount <= 0)
                    {
                        throw new ArgumentException("Amount must be positive.");

                    }

                    if (amount > 10000)
                    {
                        throw new ArgumentException("Amount exceeds maximum limit");


                    }

                    // create the transaction ID
                    string transactionID = "TID" + transactionCounter.ToString("D3") + 1;
                    //if (Transactions.ContainsKey(transactionID))
                    //{
                    //    throw new InvalidOperationException("TransactionID already exits");
                    //}

                    Transaction transaction = new Transaction
                    {
                        TransactionID = transactionID,
                        customerID = customerID,
                        amount = amount,
                        Date = DateTime.Now,
                        IsEligibleForCoupons = amount > 50

                    };
                    Transactions.Add(transactionID, transaction);



                    Customer customer = Customers[customerID];
                    customer.TotalPurchases += amount;
                    customer.LoyaltyPoints += (int) Math.Floor(amount / 10); // 1 Point per $1
                    customer.RentalCount++; //Increment for each transaction

                    if (customer.LoyaltyPoints > 100 && !customer.IsRoomOfRequirementsMember)
                    {
                        customer.IsRoomOfRequirementsMember = true;
                        Console.WriteLine("Congrat, you are now a room of requirements member.");
                    }

                    string couponResult = assignRentalBasedCoupons(customerID);
                    if (couponResult.Contains("Coupon Awarded."))
                    {
                        Console.WriteLine(couponResult);
                    }


                    Console.WriteLine($"Recorded transaction for ID={customerID}, Amount={amount:F2} with transactionID: {transactionID}");
                    
                    return $"Transaction recorded successfully for {customerID}";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to record transaction: {ex.Message}");
                    return $"Error: {ex.Message}";
                }
            }

            private static string assignRentalBasedCoupons(string customerID)
            {
                // TODO: To be implemented
                return "No coupon awarded.";
            }



            private static Dictionary<string, Customer> Customers = new Dictionary<string, Customer>();
            private static Dictionary<string, Transaction> Transactions = new Dictionary<string, Transaction>();

            private static int customerCounter = 0;
            private static int transactionCounter = 0;
            static void Main(string[] args)
            {

                while (true)
                {
                    Console.WriteLine(@"Choose an option from the list below
--------------------------------------------------------------------------
1. Add new customer
2. Record Transaction
3. Show Users
4. Show transactions");

                    int option = int.Parse(Console.ReadLine());
                    switch (option)
                    {
                        case 1:
                            string custName;
                            string custEmail;
                            Console.WriteLine("Enter customer name: ");
                            custName = Console.ReadLine();

                            Console.WriteLine("Enter customer email: ");
                            custEmail = Console.ReadLine();

                            AddNewCustomer(custName, custEmail);


                            break;

                        case 2:
                            Console.WriteLine("Enter customerID: ");
                            string customerID = Console.ReadLine();
                            Console.WriteLine("Enter transaction amount: ");
                            if (!double.TryParse(Console.ReadLine(), out double amount))
                            {
                                throw new ArgumentException("Invalid amount format.");
                            }
                            string result = RecordTransaction(customerID, amount);
                            Console.WriteLine(result);
                            break;

                        case 3:
                            Console.WriteLine("Showing current users\n\n");
                            DisplayCustomers();
                            break;

                        case 4:
                            Console.WriteLine("Showing all transactions\n\n");
                            DisplayTransaction();
                            break;

                        default:
                            Console.WriteLine("Invalid Menu Option.");
                            break;
                    }







                    

                }
            }
        }
        

        
    
}
