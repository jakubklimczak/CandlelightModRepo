﻿using System.ComponentModel.DataAnnotations;

namespace Candlelight.Core.Entities.Steam;

public class Platform
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}