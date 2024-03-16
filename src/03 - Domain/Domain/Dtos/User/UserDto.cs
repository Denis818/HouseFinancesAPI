namespace Domain.Dtos.User
{
    public record UserDto(string Email, string Password);

    public record UserInfoDto(string Email, bool IsAdmin);
}
