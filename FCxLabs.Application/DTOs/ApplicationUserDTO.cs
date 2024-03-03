#nullable disable

namespace FCxLabs.Application.DTOs
{
    public class ApplicationUserDTO
    {
        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string CPF { get; set; }

        public DateTime BirthDate { get; set; }

        public string MotherName { get; set; }

        public bool Status { get; set; }
    }
}
