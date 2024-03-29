namespace Domain.Dtos.User
{
    public class UserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public record UserInfoDto(string Email, bool IsAdmin);
}
