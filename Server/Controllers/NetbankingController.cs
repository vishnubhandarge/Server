using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Server.Models;
using Server.Models.Netbanking;
using Server.Models.Netbanking.DTOs;
using Server.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetbankingController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly BankDbContext _bankDbContext;
        // Constructor for AccountController
        public NetbankingController(BankDbContext bankDbContext, IConfiguration configuration)
        {
            _bankDbContext = bankDbContext;
            _configuration = configuration;
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
                    IsRegistered = 1,
                };

                // Save the updated customer data
                await _bankDbContext.Users.AddAsync(user);
                await _bankDbContext.SaveChangesAsync();

                // Generate a token or return a success response.
                var netbankingRegistrationResponseDTO = new NetbankingRegistrationResponseDTO
                {
                    Status = 200,
                    Message = "Registration successful. You can login to netbanking now."
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
            try
            {
                var customer = _bankDbContext.Customers.FirstOrDefault(c =>
                c.CRN == loginDTO.CRN);
                // Check if account is null.
                if (customer is null)
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
                    return BadRequest("Account is not inactive, activate account to use netbanking again.");
                }

                var user = await _bankDbContext.Users.FirstOrDefaultAsync(u =>
                u.CRN == loginDTO.CRN);

                // Check if user is null.
                if (user == null || !PasswordHasher.VerifyPasswordHash(loginDTO.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return BadRequest("Invalid login details.");
                }

                if (user != null)
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim("CRN", user.CRN.ToString()),
                    };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddHours(10),
                    signingCredentials: signIn
                    );
                    string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

                    ResponseDTO response = new ResponseDTO
                    {
                        Status = 200,
                        Message = "Login successful."
                    };
                    return Ok(new
                    {
                        Token = tokenValue,
                        response
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return BadRequest();
        }

        // Logout from netbanking
        [Authorize]
        [HttpPost("NetBankingLogout")]
        public async Task<IActionResult> NetBankingLogout()
        {

            return Ok(new ResponseDTO
            {
                Status = 200,
                Message = "Logout successful."
            });

        }

        // Forgot password
        [HttpPost("NetBankingForgotPassword")]
        public async Task<IActionResult> NetBankingForgotPassword(NetbankingForgotPasswordDTO forgotPasswordDTO)
        {
            if (forgotPasswordDTO == null)
            {
                return BadRequest("Enter all details.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Enter correct account details.");
            }

            var validAccountdetail = _bankDbContext.Customers.FirstOrDefault(c => c.AccountNumber == forgotPasswordDTO.AccountNumber && c.BirthDate == forgotPasswordDTO.BirthDate);
            if (validAccountdetail == null)
            {
                return NotFound("Invalid account details.");
            }

            var cardValidation = _bankDbContext.Cards.FirstOrDefault(c => c.CardNumber == forgotPasswordDTO.CardNumber && c.Cvv == forgotPasswordDTO.Cvv && c.Pin == forgotPasswordDTO.Pin);
            if (cardValidation == null)
            {
                return NotFound("Invalid card details.");
            }

            try
            {
                // Create password hash and salt
                PasswordHasher.CreatePasswordHash(forgotPasswordDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

                var fetchUser = _bankDbContext.Users.FirstOrDefault(c => c.CRN == validAccountdetail.CRN);
                if (fetchUser == null)
                {
                    return BadRequest("User not found.");
                }

                fetchUser.PasswordHash = passwordHash;
                fetchUser.PasswordSalt = passwordSalt;
                _bankDbContext.Users.Update(fetchUser);
                await _bankDbContext.SaveChangesAsync(); // Ensure changes are saved to the database

                return Ok(new ResponseDTO
                {
                    Status = 200,
                    Message = "Password reset successful."
                });

            }
            catch (Exception)
            {
                return BadRequest(new ResponseDTO
                {
                    Status = 500,
                    Message = "Internal Server Error."
                });
            }
        }
    }
}
