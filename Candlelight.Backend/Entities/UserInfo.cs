using System.ComponentModel.DataAnnotations;

namespace Candlelight.Backend.Entities;

public class UserInfo
{
    public Guid Id { get; set; }

    [MaxLength(30)]
    public required string UserName { get; set; }

    [MaxLength(255)]
    public required string UserEmail { get; set; }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public required string PasswordHash { get; set; }

    public required DateTime Created { get; set; }

    public required DateTime LastUpdated { get; set; }
}