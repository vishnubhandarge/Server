using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;
using Server.Models.DTOs;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly BankDbContext _bankDbContext;
        public static class AccountDetailsGenerator
        {
            private static long _currentAccountNumber = 04135120000000;

            public static long GetNextAccountNumber()
            {
                return _currentAccountNumber++;
            }

            private static long _crn = 02200000000;

            public static long GetNextCRN()
            {
                return _crn++;
            }


        }

        //public long number = 04135120000001;

        public AccountController(BankDbContext bankDbContext)
        {
            _bankDbContext = bankDbContext;
        }

        [HttpPost("OpenAccount")]
        public async Task<IActionResult> OpenAccount(CustomerCreateAccountDTO customer)
        {


            static Customer CreateNewCustomer(CustomerCreateAccountDTO customer)
            {
                return new Customer
                {
                    //Personal
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    BirthDate = customer.BirthDate.Date,
                    Mobile = customer.Mobile,
                    Email = customer.Email,
                    NormalizedEmail = customer.Email.ToUpper(),
                    //Address
                    HouseNo = customer.HouseNo,
                    AddressLine1 = customer.AddressLine1,
                    AddressLine2 = customer.AddressLine2,
                    Taluka = customer.Taluka,
                    City = customer.City,
                    State = customer.State,
                    Country = customer.Country,
                    PinCode = customer.PinCode,
                    //Banking
                    AccountNumber = AccountDetailsGenerator.GetNextAccountNumber(),
                    CRN = AccountDetailsGenerator.GetNextCRN(),
                    AccountType = customer.AccountType,
                    Branch = customer.Taluka,
                    IfscCode = customer.Taluka + customer.PinCode,
                    OpeningDate = DateTime.Now.Date,
                    AccountBalance = 0,
                    IsActive = true,
                    IsClosed = false,
                    //Nominee details
                    NomineeName = customer.NomineeName,
                    RelationWithNominee = customer.RelationWithNominee,
                    NomineeDOB = customer.NomineeDOB
                };
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (customer == null)
            {
                return BadRequest("Model is empty");
            }

            var accountExists = _bankDbContext.Customers.Any(c => c.Mobile == customer.Mobile && c.Email == customer.Email);
            if (accountExists)
            {
                return BadRequest($"Account already exists. Login or Signup to continue.\nMobile:{customer.Mobile}\tEmail: {customer.Email}");
            }

            // Check if account exists and is closed
            var accountExistsAndClosed = _bankDbContext.Customers.Any(c => c.Email == customer.Email && c.Mobile == customer.Mobile && c.IsClosed == true);

            // Create the customer object
            var newCustomer = CreateNewCustomer(customer);

            // Add new customer and save changes
            await _bankDbContext.AddAsync(newCustomer);
            await _bankDbContext.SaveChangesAsync();

            return Ok(CreateAccountSuccessMessage(newCustomer));

            string CreateAccountSuccessMessage(Customer customer)
            {
                return $"Account created successfully. Here is your account details\n" +
                       $"AccountNumber: {customer.AccountNumber}\tIfscCode: {customer.IfscCode}\n" +
                       $"Branch: {customer.Branch}\tAccountType: {customer.AccountType}\tCRN: {customer.CRN}\n" +
                       $"OpeningDate: {customer.OpeningDate}\tAccountBalance: {customer.AccountBalance}\n" +
                       $"Nominee: {customer.NomineeName}\tRelation: {customer.RelationWithNominee}\n" +
                       $"NomineeDOB: {customer.NomineeDOB}";
            }

        }


        [HttpGet("GetAccountDetails")]
        public async Task<IActionResult> GetAccountDetails(GetAccountDetails accountDetails)
        {
            if (accountDetails == null)
            {
                return NotFound("Enter all details.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Enter correct account details.");
            }

            // Check if account is closed.
            var customer = await _bankDbContext.Customers.FirstOrDefaultAsync(c => c.AccountNumber == accountDetails.AccountNumber && c.IsClosed == true);
            if (customer != null)
            {
                return Ok($"Account is closed. Please open a new account.");
            }

            // Return details if account is active
            var isAvailable = await _bankDbContext.Customers.FirstOrDefaultAsync(c => c.AccountNumber == accountDetails.AccountNumber && c.IsClosed == false && c.IsActive == true && (c.CRN == accountDetails.CRN || c.BirthDate == accountDetails.BirthDate));
            if (isAvailable != null)
            {
                return Ok($"Account details found.\n" +
                           $"AccountNumber: {isAvailable.AccountNumber}\tIfscCode: {isAvailable.IfscCode}\n" +
                           $"Branch: {isAvailable.Branch}\tAccountType: {isAvailable.AccountType}\tCRN: {isAvailable.CRN}\n" +
                           $"OpeningDate: {isAvailable.OpeningDate}\tAccountBalance: {isAvailable.AccountBalance}\n" +
                           $"Nominee: {isAvailable.NomineeName}\tRelation: {isAvailable.RelationWithNominee}\n" +
                           $"NomineeDOB: {isAvailable.NomineeDOB}");
            }
            else
            {
                return NotFound("No account found with the provided details.");
            }

        }
    }
}