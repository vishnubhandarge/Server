using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Server.Data;
using Server.Models;
using Server.Models.Account;
using Server.Models.Account.DTOs;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly BankDbContext _bankDbContext;
        // Constructor for AccountController
        public AccountController(BankDbContext bankDbContext)
        {
            _bankDbContext = bankDbContext;
        }
        // Class and constructor for generating account details
        public class AccountDetailsGenerator
        {
            private static BankDbContext _context;
            // Constructor for AccountDetailsGenerator
            public AccountDetailsGenerator(BankDbContext context)
            {
                _context = context;
            }

            // Get next account number
            public long GetNextAccountNumber()
            {
                var latestAccount = _context.Customers
                                            .OrderByDescending(c => c.AccountNumber)
                                            .FirstOrDefault();
                return latestAccount != null ? latestAccount.AccountNumber + 1 : 4135120000000;
            }
            // Get next CRN
            internal long GetNextCRN()
            {
                var latestCustomer = _context.Customers
                                             .OrderByDescending(c => c.CRN)
                                             .FirstOrDefault();
                return latestCustomer != null ? latestCustomer.CRN + 1 : 2200000000;
            }

            // Generate a unique card number
            internal long GenerateCardNumber()
            {
                // Implement your logic to generate a unique card number
                Random random = new Random();
                long part1 = random.NextInt64(10000000, 99999999);
                long part2 = random.NextInt64(10000000, 99999999);
                string cardNumberString = part1.ToString() + part2.ToString();
                long cardNumber = long.Parse(cardNumberString);
                return cardNumber;

            }
            // Generate a CVV
            internal int GenerateCvv()
            {
                // Implement your logic to generate a CVV
                return new Random().Next(100, 999); // Generates a random 3-digit CVV
            }
            // Generate expiry date
            internal string GenerateExpiryDate()
            {
                // Set expiry date to 8 years from now
                DateTime expiry = DateTime.Now.AddYears(8); // Example: 8 years from now return expiry.ToString("MM/yy");
                return expiry.ToString("MM/yy");
            }
            // Generate card type
            internal string GenerateCardType()
            {
                // Array of card types
                string[] cardTypes = { "Platinum", "MoneyBack", "Millenia", "Coral", "Gold", "Global", "Rubyx", "Sapphire", "Silver", "Titanium", "Emeralde", "Select", "Premium", "Signature", "Infinite", "Reward" };
                // Select a random card type
                return cardTypes[new Random().Next(cardTypes.Length)];
            }
            // Generate card issuer
            internal string CardIssuer()
            {
                string[] cardIssuers = { "Visa", "MasterCard", "Rupay", "Diners Club", "American Express", "JCB EXPRESS", "Maestro", "Discover", "UnionPay" };
                return cardIssuers[new Random().Next(cardIssuers.Length)];
            }
        }

        // Create new account
        [HttpPost("OpenAccount")]
        public async Task<IActionResult> OpenAccount(CustomerCreateAccountDTO customer)
        {
            // Check if model is empty
            if (customer == null)
            {
                return BadRequest("Model is empty");
            }

            // Check if model is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if account exists
            var accountExists = _bankDbContext.Customers.Any(c => c.Mobile == customer.Mobile && c.Email == customer.Email);
            if (accountExists)
            {
                return BadRequest($"Account already exists. Login or Signup to continue.\nMobile:{customer.Mobile}\tEmail: {customer.Email}");
            }

            // Check if account exists and is closed
            var accountExistsAndClosed = _bankDbContext.Customers.Any(c => c.Email == customer.Email && c.Mobile == customer.Mobile && c.IsClosed == true);
            if (accountExistsAndClosed)
            {
                return BadRequest($"Account already exists and closed. Activate again to continue.\nMobile:{customer.Mobile}\tEmail: {customer.Email}");
            }

            // Generate account details
            var accountDetailsGenerator = new AccountDetailsGenerator(_bankDbContext);

            // Create the customer object
            var newCustomer = new Customer
            {
                // Personal
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                BirthDate = customer.BirthDate.Date,
                Mobile = customer.Mobile,
                Email = customer.Email,
                NormalizedEmail = customer.Email.ToUpper(),
                // Address
                HouseNo = customer.HouseNo,
                AddressLine1 = customer.AddressLine1,
                AddressLine2 = customer.AddressLine2,
                City = customer.City,
                District = customer.District,
                State = customer.State,
                Country = customer.Country,
                PinCode = customer.PinCode,
                // Banking
                AccountNumber = accountDetailsGenerator.GetNextAccountNumber(),
                CRN = accountDetailsGenerator.GetNextCRN(),
                AccountType = customer.AccountType,
                Branch = customer.City,
                IfscCode = customer.City + customer.PinCode,
                OpeningDate = DateTime.Now.Date,
                AccountBalance = 0,
                IsActive = true,
                IsClosed = false,
                // Nominee details
                NomineeName = customer.NomineeName,
                RelationWithNominee = customer.RelationWithNominee,
                NomineeDOB = customer.NomineeDOB,
                Cards = new List<Card>() // Initialize the card list
            };

            // Create and add a new card to the customer
            var newCard = new Card
            {
                CardNumber = accountDetailsGenerator.GenerateCardNumber(),
                CardIssuer = accountDetailsGenerator.CardIssuer(),
                CardType = accountDetailsGenerator.GenerateCardType(),
                ExpiryDate = accountDetailsGenerator.GenerateExpiryDate(),
                Cvv = accountDetailsGenerator.GenerateCvv(),
                NameOnCard = $"{newCustomer.FirstName} {newCustomer.LastName}",
                IsActive = true,
                AccountNumber = newCustomer.AccountNumber
            };

            newCustomer.Cards.Add(newCard);

            // Add new customer and save changes
            await _bankDbContext.AddAsync(newCustomer);
            await _bankDbContext.SaveChangesAsync();

            // Return response with newly generated account details
            var response = new CustomerCreateAccountResponseDTO
            {
                //Account details
                AccountNumber = newCustomer.AccountNumber,
                IfscCode = newCustomer.IfscCode,
                CRN = newCustomer.CRN,
                Branch = newCustomer.Branch,
                AccountType = newCustomer.AccountType,
                //Card details
                CardNumber = newCard.CardNumber,
                CardIssuer = newCard.CardIssuer,
                CardType = newCard.CardType,
                Cvv = newCard.Cvv,
                ExpiryDate = newCard.ExpiryDate,
                NameOnCard = newCard.NameOnCard,
                IsActive = true,
            };
            return Ok(response);
        }

        //Get account details
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
                return Ok("Account is closed. Please open a new account.");
            }

            // Return details if account is active
            var isAvailable = await _bankDbContext.Customers.FirstOrDefaultAsync(c => c.AccountNumber == accountDetails.AccountNumber && c.IsClosed == false && c.IsActive == true && (c.CRN == accountDetails.CRN || c.BirthDate == accountDetails.BirthDate));
            if (isAvailable != null)
            {
                var respone = new GetAccountDetailsResponseDTO
                {
                    AccountNumber = isAvailable.AccountNumber,
                    Branch = isAvailable.Branch,
                    IfscCode = isAvailable.IfscCode,
                    CRN = isAvailable.CRN,
                    AccountType = isAvailable.AccountType,
                    OpeningDate = isAvailable.OpeningDate,
                    AccountBalance = isAvailable.AccountBalance,
                    NomineeName = isAvailable.NomineeName,
                    NomineeDOB = isAvailable.NomineeDOB,
                    RelationWithNominee = isAvailable.RelationWithNominee,
                };
                return Ok(respone);
            }
            else
            {
                return NotFound("No account found with the provided details.");
            }
        }

        // Activate account
        [HttpPost("ActivateAccount")]
        public async Task<IActionResult> ActivateAccount(AccountActivateDTO activateAccount)
        {
            if(activateAccount is null)
            {
                return BadRequest("Fill all the details.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Enter correct credentils.");
            }
            var customer = _bankDbContext.Customers.FirstOrDefault(c =>
            c.AccountNumber == activateAccount.AccountNumber &&
            c.BirthDate == activateAccount.BirthDate &&
            c.CRN == activateAccount.CRN);

            if (customer is null)
            {
                return NotFound("No account found with the provided details.");
            }
            else if (customer.IsClosed == true)
            {
                return BadRequest("Account is closed.");
            }
            else if (customer.IsActive == true)
            {
                return BadRequest("Account is already active try to login.");
            }
            else if (customer is not null)
            {
                customer.IsActive = true;
                customer.IsClosed = false;
                await _bankDbContext.SaveChangesAsync();
                ResponseDTO successResponse = new ResponseDTO
                {
                    Status = 200,
                    Message = "Account activated successfully."
                };
                return Ok(successResponse);
            }
                
            ResponseDTO response = new ResponseDTO
                {
                    Status = 500,
                    Message = "Error"
                };
            return BadRequest(response);
        }

        // Close account
        [HttpPost("CloseAccount")]
        public async Task<IActionResult> CloseAccount(CloseAccountDTO closeAccount)
        {
            if (closeAccount == null)
            {
                return BadRequest("Fill all the details.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Enter correct credentils.");
            }

            var customer = await _bankDbContext.Customers.FirstOrDefaultAsync(c =>
                c.AccountNumber == closeAccount.AccountNumber &&
                c.BirthDate == closeAccount.BirthDate &&
                c.CRN == closeAccount.CRN);

            if (customer is null)
            {
                return NotFound("No account found with the provided details.");
            }

            else if (customer.IsClosed == true)
            {
                return BadRequest("Account is already closed.");
            }
            else if(customer.IsActive is true)
            {
                return BadRequest("Account is not active activate it first.");
            }
            else if (customer is not null)
            {
                customer.IsActive = false;
                customer.IsClosed = true;
                await _bankDbContext.SaveChangesAsync();
                ResponseDTO response = new ResponseDTO
                {
                    Status = 200,
                    Message = "Account closed."
                };
                return Ok(response);
            }
            else
            {
                ResponseDTO response = new ResponseDTO
                {
                    Status = 500,
                    Message = "Error"
                };

                return BadRequest(response);
            }
        }
    }
}
