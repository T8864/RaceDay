namespace RaceDay.MVC.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = "";
        public string Role { get; set; } = "";
        public int UserID { get; set; }
        public string FirstName { get; set; } = "";
    }
}