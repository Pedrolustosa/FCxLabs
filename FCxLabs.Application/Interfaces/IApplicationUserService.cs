using FCxLabs.Application.DTOs;
using System.Data;

namespace FCxLabs.Application.Interfaces
{
    public interface IApplicationUserService
    {
        Task<List<ApplicationUserDTO>> GetAll(int pageNumber, int pageQuantity);

        Task<ApplicationUserDTO> GetById(string id);

        Task<ApplicationUserDTO> GetUserName(string userName);

        Task<IEnumerable<ApplicationUserFilterDTO>> GetFilter(string email, string name, string birthDate, string motherName);

        Task<bool> Register(ApplicationUserRegisterDTO applicationUserRegisterDTO, string role);

        Task<ApplicationUserUpdateDTO> UpdateAccount(ApplicationUserUpdateDTO applicationUserUpdateDTO);

        string ExportToPdf(DataTable data, string fileName);

        DataTable GetPersonalData();

        bool ValidateCPF(string cpf);

        bool ExistingCPF(string cpf);
    }
}
