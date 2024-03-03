using AutoMapper;
using System.Data;
using iText.Layout;
using iText.Kernel.Pdf;
using iText.Kernel.Geom;
using iText.Kernel.Font;
using iText.Kernel.Colors;
using iText.Layout.Element;
using System.Data.SqlClient;
using iText.IO.Font.Constants;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using FCxLabs.Domain.Entities;
using FCxLabs.Application.DTOs;
using FCxLabs.Domain.Interfaces;
using FCxLabs.Application.Interfaces;

#nullable disable
namespace FCxLabs.Infra.Data.Service
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IApplicationUserRepository _userRepository;

        private readonly IMapper _mapper;

        private readonly IConfiguration _configuration;

        public ApplicationUserService(UserManager<ApplicationUser> userManager,
                                   RoleManager<IdentityRole> roleManager,
                                   IApplicationUserRepository applicationUserRepository,
                                   IMapper mapper,
                                   IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = applicationUserRepository;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<List<ApplicationUserDTO>> GetAll(int pageNumber, int pageQuantity)
        {
            var applicationUser = await _userRepository.GetAll(pageNumber, pageQuantity);
            return _mapper.Map<List<ApplicationUserDTO>>(applicationUser);
        }

        public async Task<ApplicationUserDTO> GetById(string id)
        {
            var applicationUser = await _userRepository.GetById(id);
            return _mapper.Map<ApplicationUserDTO>(applicationUser);
        }

        public async Task<ApplicationUserDTO> GetUserName(string userName)
        {
            var user = await _userRepository.GetUserName(userName) ?? throw new Exception("This user not exits!");
            var applicationUserDTO = _mapper.Map<ApplicationUserDTO>(user);
            return applicationUserDTO;
        }

        public async Task<IEnumerable<ApplicationUserFilterDTO>> GetFilter(string email, string name, string birthDate, string motherName)
        {
            var applicationUser = await _userRepository.GetFilter(email, name, birthDate, motherName);
            return _mapper.Map<IEnumerable<ApplicationUserFilterDTO>>(applicationUser);
        }

        public async Task<bool> Register(ApplicationUserRegisterDTO applicationUserRegisterDTO, string role)
        {
            var applicationUser = _mapper.Map<ApplicationUser>(applicationUserRegisterDTO);
            var isCPF = ValidateCPF(applicationUser.CPF);
            var existingCPF = ExistingCPF(applicationUser.CPF);
            if (isCPF && !existingCPF)
            {
                var userExist = await _userManager.FindByEmailAsync(applicationUser.Email);
                if (userExist != null) throw new Exception("Email already exists.");
                if (await _roleManager.RoleExistsAsync(role))
                {
                    var result = await _userManager.CreateAsync(applicationUser, applicationUserRegisterDTO.Password);
                    if (!result.Succeeded) throw new Exception("Error!");
                    await _userManager.AddToRoleAsync(applicationUser, role);
                    return result.Succeeded;
                }
                else
                    throw new Exception("Please, choose a role for this user!");
            }
            else
            {
                throw new Exception("CPF or Age Invalid");
            }
        }

        public async Task<ApplicationUserUpdateDTO> UpdateAccount(ApplicationUserUpdateDTO applicationUserUpdateDTO)
        {
            var user = await _userRepository.GetUserName(applicationUserUpdateDTO.UserName) ?? throw new Exception("This user not exits!");
            applicationUserUpdateDTO.Id = user.Id;
            user.DateAlteration = DateTime.Now;
            var existingCPF = ExistingCPF(applicationUserUpdateDTO.CPF);
            _mapper.Map(applicationUserUpdateDTO, user);
            if (existingCPF && applicationUserUpdateDTO.Password != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, applicationUserUpdateDTO.Password);
            }
            await _userManager.UpdateAsync(user);
            var userRetorno = await _userRepository.GetUserName(user.UserName);
            return _mapper.Map<ApplicationUserUpdateDTO>(userRetorno);
        }

        public DataTable GetPersonalData()
        {
            DataTable data = new();
            using (SqlConnection connection = new(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new(_userRepository.GetDataPDF(), connection);
                SqlDataAdapter adapter = new(command);
                adapter.Fill(data);
            }
            return data;
        }

        public string ExportToPdf(DataTable data, string fileName)
        {
            string filePath = System.IO.Path.Combine(@"C:\Users\pedro\Downloads", fileName + ".pdf");
            PdfWriter writer = new(new FileStream(filePath, FileMode.Create));
            PdfDocument pdf = new(writer);
            Document document = new(pdf, PageSize.A4);
            document.SetMargins(30, 30, 30, 30);
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
            Paragraph title = new Paragraph("List of Users").SetFont(font);
            title.SetFontSize(24);
            title.SetFontColor(ColorConstants.DARK_GRAY);
            title.SetTextAlignment(TextAlignment.CENTER);
            Table table = new(data.Columns.Count);
            table.SetWidth(UnitValue.CreatePercentValue(100));
            foreach (DataColumn column in data.Columns)
            {
                Cell cell = new Cell().Add(new Paragraph(column.ColumnName));
                cell.SetTextAlignment(TextAlignment.CENTER);
                cell.SetBackgroundColor(ColorConstants.WHITE);
                table.AddHeaderCell(cell);
            }
            foreach (DataRow row in data.Rows)
            {
                foreach (object item in row.ItemArray)
                {
                    Cell cell = new Cell().Add(new Paragraph(item.ToString()));
                    cell.SetTextAlignment(TextAlignment.CENTER);
                    table.AddCell(cell);
                }
            }
            document.Add(title);
            document.Add(table);
            document.Close();
            return filePath;
        }

        public bool ValidateCPF(string cpf)
        {
            int[] multiplier1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            cpf = cpf.Trim().Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;

            string tempCpf = cpf[..9];
            int sum = 0;

            for (int i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier1[i];

            int remainder = sum % 11;
            int firstCheckDigit = (remainder < 2) ? 0 : 11 - remainder;

            tempCpf += firstCheckDigit.ToString();
            sum = 0;

            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multiplier2[i];

            remainder = sum % 11;
            int secondCheckDigit = (remainder < 2) ? 0 : 11 - remainder;

            string calculatedDigits = $"{firstCheckDigit}{secondCheckDigit}";

            return cpf.EndsWith(calculatedDigits);
        }

        public bool ExistingCPF(string cpf) { return _userRepository.ExistingCPF(cpf); }
    }
}
