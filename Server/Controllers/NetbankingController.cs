
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models.Netbanking.DTOs;
using Server.Models.Netbanking;
using Server.Models;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetbankingController : ControllerBase
    {

        private readonly BankDbContext _bankDbContext;
        // Constructor for AccountController
        public NetbankingController(BankDbContext bankDbContext)
        {
            _bankDbContext = bankDbContext;
        }

        // Register to netbanking
        [HttpPost("NetBankingRegistration")]
        public async Task<IActionResult> NetBankingRegistration(NetbankingRegistrationDTO registrationDTO)
        {
            // Check if model is empty
            if (registrationDTO == null)
            {
                return BadRequest("Enter all details.");
            }

            // Check if model is valid
            if (!ModelState.IsValid)
            {
                return BadRequest("Enter correct account details.");
            }
            try
            {
                // Fetch customer data including related cards
                var customer = await _bankDbContext.Customers
                    .FirstOrDefaultAsync(c =>
                        c.CRN == registrationDTO.CRN &&
                        c.AccountNumber == registrationDTO.AccountNumber);
                // Check if Account is closed.
                if (customer.IsClosed == true) 
                {
                    return BadRequest("Account is closed");
                }
                // Check if account is active
                else if (customer.IsActive == false)
                {
                    return BadRequest("Account is not active");
                }
                // Check if customer is null.
                if (customer == null)
                {
                    return NotFound("No account found with the provided details.");
                }

                // Verify card details
                var card = await _bankDbContext.Cards.FirstOrDefaultAsync(c =>
                    c.CardNumber == registrationDTO.CardNumber);
                // Check if card is null.
                if (card == null)
                {
                    return NotFound("Invalid card details.");
                }

                // Create password hash and salt
                PasswordHasher.CreatePasswordHash(registrationDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

                // Update customer with netbanking details
                var user = new User
                {
                    CRN = customer.CRN,
                    AccountNumber = customer.AccountNumber,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    CardNumber = card.CardNumber,
                };

                // Save the updated customer data
                await _bankDbContext.Users.AddAsync(user);
                await _bankDbContext.SaveChangesAsync();

                // Generate a token or return a success response.
                var netbankingRegistrationResponseDTO = new NetbankingRegistrationResponseDTO
                {
                    Status = 200,
                    Message = "Registration successful. You can log in to netbanking now."
                };
                // Return success response
                return Ok(netbankingRegistrationResponseDTO);
            }
            catch (Exception ex)
            {
                NetbankingRegistrationResponseDTO error = new NetbankingRegistrationResponseDTO
                {
                    Status = 400,
                    Message = "Check Provided details are correct and try again."
                };
                return BadRequest(error);
            }
        }

        // Login to netbanking
        [HttpPost("NetBankingLogin")]
        public async Task<IActionResult> NetBankingLogin(NetbankingLoginDTO loginDTO)
        {
            // Check if model is empty
            if (loginDTO == null)
            {
                return BadRequest("Enter all details.");
            }
            // Check if model state is valid
            else if (!ModelState.IsValid)
            {
                return BadRequest("Enter correct account details.");
            }

            var customer = _bankDbContext.Customers.FirstOrDefault(c => 
            c.CRN == loginDTO.CRN);
            // Check if account is null.
            if(customer is null)
            {
                return NotFound("No account found with the provided details.");
            }
            // Check if account is closed.
            else if (customer.IsClosed == true) 
            {
                return BadRequest("Account is closed.");
            }
            // Check if account is active.
            else if (customer.IsActive == false)
            {
                return BadRequest("Account is not active, activate account to use netbanking again.");
            }

            var user = await _bankDbContext.Users.FirstOrDefaultAsync(u =>
            u.CRN == loginDTO.CRN); 

            // Check if user is null.
            if (user == null || !PasswordHasher.VerifyPasswordHash(loginDTO.Password, user.PasswordHash, user.PasswordSalt))
            {
                return BadRequest("Invalid login details.");
            }
            // Generate a token or return a success response
            ResponseDTO response = new ResponseDTO
            {
                Status = 200,
                Message = "Login successful."
            };
            return Ok(response);
        }

        // Logout from netbanking
        [HttpPost("NetBankingLogout")]
        public async Task<IActionResult> NetBankingLogout(NetbankingLogoutDTO logoutDTO)
        {
            return Ok("Logout successful.");
        }
    }
}
