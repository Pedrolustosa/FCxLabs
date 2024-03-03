using FCxLabs.Domain.Entities;

namespace FCxLabs.Domain.Interfaces
{
    public interface IApplicationUserRepository
    {
        string GetDataPDF();

        Task<List<ApplicationUser>> GetAll(int pageNumber, int pageQuantity);

        Task<ApplicationUser> GetById(string id);

        bool GetStatusUser(string userName);

        Task<ApplicationUser> GetUserName(string userName);

        Task<List<ApplicationUser>> GetFilter(string email, string name, string birthDate, string motherName);

        bool ExistingCPF(string cpf);
    }
}
