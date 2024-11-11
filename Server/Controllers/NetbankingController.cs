
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

        [HttpPost("NetBankingRegistration")]
        public async Task<IActionResult> NetBankingRegistration(NetbankingRegistrationDTO registrationDTO)
        {
            if (registrationDTO == null)
            {
                return BadRequest("Enter all details.");
            }

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

                if (customer == null)
                {
                    return NotFound("No account found with the provided details.");
                }

                // Verify card details
                var card = await _bankDbContext.Cards.FirstOrDefaultAsync(c =>
                    c.CardNumber == registrationDTO.CardNumber);

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

                var netbankingRegistrationResponseDTO = new NetbankingRegistrationResponseDTO
                {
                    Status = 200,
                    Message = "Registration successful. You can log in to netbanking now."
                };

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

        [HttpPost("NetBankingLogin")]
        public async Task<IActionResult> NetBankingLogin(NetbankingLoginDTO loginDTO)
        {
            if (loginDTO == null)
            {
                return BadRequest("Enter all details.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest("Enter correct account details.");
            }
            var user = await _bankDbContext.Users.FirstOrDefaultAsync(u => 
            u.CRN == loginDTO.CRN);

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
    }
}
