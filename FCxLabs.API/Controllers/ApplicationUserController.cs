using System.Data;
using Microsoft.AspNetCore.Mvc;
using FCxLabs.API.Models;
using Microsoft.AspNetCore.Authorization;
using FCxLabs.Application.DTOs;
using FCxLabs.Application.Interfaces;

#nullable disable
namespace FCxLabs.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private readonly IApplicationUserService _applicationUserService;

        public ApplicationUserController(IApplicationUserService applicationUserService)
        {
            _applicationUserService = applicationUserService;
        }

        [HttpGet("GetAllUsers")]
        [AllowAnonymous]
        public async Task<List<ApplicationUserDTO>> GetAll(int pageNumber, int pageQuantity) => await _applicationUserService.GetAll(pageNumber, pageQuantity);

        [HttpGet("GetFilter")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFilter([FromQuery] ApplicationUserFilterDTO applicationUserFilterDTO)
        {
            var result = await _applicationUserService.GetFilter(applicationUserFilterDTO.Email, 
                                                                 applicationUserFilterDTO.Name,
                                                                 applicationUserFilterDTO.BirthDate.ToString(),
                                                                 applicationUserFilterDTO.MotherName);
            return Ok(new {Message = "Total Users: " + result.ToList().Count, Data = result});
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserToken>> Register([FromBody] ApplicationUserRegisterDTO applicationUserRegisterDTO, string role)
        {
            var result = await _applicationUserService.Register(applicationUserRegisterDTO, role);
            if (result)
                return Ok($"User {applicationUserRegisterDTO.Email} was created with success!");
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Login attempt.");
                return BadRequest(ModelState);
            }
        }

        [HttpPut("UpdateUser")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateUser(ApplicationUserUpdateDTO applicationUserUpdateDTO)
        {
            _ = await _applicationUserService.GetUserName(applicationUserUpdateDTO.UserName) ?? throw new Exception("This user not exits!");
            var applicationUserUpdate = await _applicationUserService.UpdateAccount(applicationUserUpdateDTO);
            if (applicationUserUpdate is null) return NoContent();
            return Ok(new { email = applicationUserUpdate.Email });
        }

        [HttpGet("PDF")]
        [AllowAnonymous]
        public IActionResult GetPersonalData()
        {
            DataTable data = _applicationUserService.GetPersonalData();
            string pdfPath = _applicationUserService.ExportToPdf(data, "ListUsers-" + DateTime.Now.ToString("dd-MM-yyyy"));
            var pdfStream = System.IO.File.OpenRead(pdfPath);
            return File(pdfStream, "application/pdf", "ListUsers-" + DateTime.Now.ToString("dd-MM-yyyy") + ".pdf");
        }
    }
}
