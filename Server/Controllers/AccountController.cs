using Microsoft.AspNetCore.Mvc;
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
        public static class AccountNumberGenerator
        {
            private static long _currentAccountNumber = 04135120000000;

            public static long GetNextAccountNumber()
            {
                return _currentAccountNumber++;
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
                return BadRequest($"Account already exists. Login or Signup to continue.\nMobile:{customer.Mobile}\tIfscCode: {customer.Email}");
            }

            var newCostomer = new Customer
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
                AccountNumber = AccountNumberGenerator.GetNextAccountNumber(),
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

            await _bankDbContext.AddAsync(newCostomer);
            await _bankDbContext.SaveChangesAsync();

            return Ok($"Account created successfully. Here is your account details\nAccountNumber: {newCostomer.AccountNumber}\tIfscCode: {newCostomer.IfscCode}\nBranch: {newCostomer.Branch}\tAccountType: {newCostomer.AccountType}\nOpeningDate: {newCostomer.OpeningDate}\tAccountBalance: {newCostomer.AccountBalance}\nNominee: {newCostomer.NomineeName}\tRelation: {newCostomer.RelationWithNominee}\nNomineeDOB: {newCostomer.NomineeDOB}");
        }

        [HttpPatch]
        public async Task<IActionResult> DeactivateAccount(DeactivateAccountDTO deactivate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (deactivate == null)
            {
                return BadRequest();
            }


            var accountExists = _bankDbContext.Customers.Any(c => c.AccountNumber == deactivate.AccountNumber);
            if (accountExists)
            {

            }
            return Ok(accountExists);
        }
    }
}