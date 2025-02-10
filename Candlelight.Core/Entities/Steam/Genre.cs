﻿using System.ComponentModel.DataAnnotations;

namespace Candlelight.Core.Entities.Steam;

public class Genre
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}