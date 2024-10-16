using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public class AuthorViewModel
{
    [Required]
    [Display(Name = "Name")]
    [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 5)]
    public string? Name { get; set; }

    [Required]
    [Display(Name = "Description")]
    [StringLength(1000, ErrorMessage = "The {0} must be at max {1} characters long.")]
    public string? Description { get; set; }
}