using Microsoft.EntityFrameworkCore;
using FCxLabs.Domain.Entities;
using FCxLabs.Domain.Interfaces;
using FCxLabs.Infra.Data.Context;

#nullable disable
namespace FCxLabs.Infra.Data.Repository
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) => _context = context;

        public async Task<List<ApplicationUser>> GetAll(int pageNumber, int pageQuantity) => await _context.Users.Skip((pageNumber -1) * pageQuantity)
                                                                                                                 .Take(pageQuantity).ToListAsync();

        public async Task<ApplicationUser> GetUserName(string userName) => await _context.Users.SingleOrDefaultAsync(x => x.UserName == userName);

        public bool GetStatusUser(string userName) => _context.Users.Any(x => x.UserName == userName && x.Status == true);

        public async Task<ApplicationUser> GetById(string id) => await _context.Users.FindAsync(id);

        public async Task<List<ApplicationUser>> GetFilter(string email, string name, string birthDate, string motherName)
        {
            return await _context.Users.Where(u => u.Email == email ||
                                                   u.Name == name ||
                                                   u.BirthDate.ToString() == birthDate ||
                                                   u.MotherName == motherName).ToListAsync();
        }

        public string GetDataPDF() => @"SELECT Email, Name, CPF, Age, MotherName, PhoneNumber, Status FROM AspNetUsers";
        
        public bool ExistingCPF(string cpf) => _context.Users.Any(u => u.CPF == cpf);
    }
}
