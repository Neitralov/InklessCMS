using System.ComponentModel.DataAnnotations;

namespace Database.Migrator;

public sealed class AdminAccountOptions
{
    public const string Section = "Admin";
    
    [Required]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}