namespace FinalProject.Models
{
    public class Users
    {
        public int Id { get; set; }
        public int AccountanId { get; set; }
        public int LoanID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public double Sallary { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public bool IsBlocked = false;
    }
}
