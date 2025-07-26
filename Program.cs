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
    internal class Program
    {

        private static Dictionary<string, Customer> Customers = new Dictionary<string, Customer>();
        private static int customerCounter = 1;
        static void Main(string[] args)
        {
            string custName;
            string custEmail;
            Console.WriteLine("Enter customer name: ");
            custName = Console.ReadLine();

            Console.WriteLine("Enter customer email: ");
            custEmail = Console.ReadLine();



            //if (!Customers.ContainsKey(custName))
            //{
            //    Console.WriteLine("Non existent customer, adding customer.");



            //}
            string result = AddNewCustomer(custName, custEmail);
            if (result.Contains("customer added successfully."))
            {
                string customerID = result.Split(':')[0].Trim(); // Extract the customerID
                if (Customers.ContainsKey(customerID))
                {
                    Customer customer = Customers[customerID];
                    Console.WriteLine($"Customer found: ID:={customer.CustomerID}, Name={customer.Name}, Email={customer.Email}");
                    Console.WriteLine($"Customers: {customerCounter}");
                }
            }
            else
            {
                Console.WriteLine("Failed to add customer: " + result);
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

                // create a unique customerID
                string customerID = "C" + customerCounter.ToString("D3");
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

            } catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return $"Error: {ex.Message}";

            }


            
        }
    }
}
