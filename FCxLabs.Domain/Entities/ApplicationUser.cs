using Microsoft.AspNetCore.Identity;
using FCxLabs.Domain.Validation;

#nullable disable 
namespace FCxLabs.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() { }

        public ApplicationUser(string email, string userName, string phoneNumber,
            string name, string cPF, DateTime birthDate, string motherName, bool status)
        {
            ValidateDomain(email, userName, phoneNumber, name, cPF, birthDate, motherName, status);
        }

        public void ValidateDomain(string email, string userName, string phoneNumber,
            string name, string cPF, DateTime birthDate, string motherName, bool status)
        {
            DomainExceptionValidation.When(cPF.Length != 11, "CPF cannot be shorter or longer than 11 characters!");
            DomainExceptionValidation.When((DateTime.Now.Year - birthDate.Year) < 18, "The User must not be under the age of eighteen");

            Email = email;
            UserName = userName;
            PhoneNumber = phoneNumber;
            Name = name;
            CPF = cPF;
            BirthDate = birthDate;
            DateInsert = DateTime.Now;
            MotherName = motherName;
            Status = status;
        }

        public string Name { get; set; }

        public string CPF { get; set; }

        public DateTime BirthDate { get; set; }

        public DateTime DateInsert { get; set; }

        public DateTime DateAlteration { get; set; }

        public string MotherName { get; set; }

        public bool Status { get; set; }
    }
}