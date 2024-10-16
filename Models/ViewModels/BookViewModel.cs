using System.ComponentModel.DataAnnotations;

namespace BookStore.Models;

public class BookViewModel
{
    [Required]
    [Display(Name = "Title")]
    [StringLength(100, ErrorMessage = "The {0} must be at max {1} characters long.")]
    public string? Title { get; set; }

    [Required]
    [Display(Name = "Description")]
    [StringLength(1000, ErrorMessage = "The {0} must be at max {1} characters long.")]
    public string? Description { get; set; }

    [Required]
    [Display(Name = "Price")]
    [Range(0.01, 100_000.0)]
    public decimal Price { get; set; }

    [Required]
    [Display(Name = "Published Year")]
    [Range(1000, 2500)]
    public int PublishedYear { get; set; }

    [Required]
    [Display(Name = "Publisher")]
    public string? Publisher { get; set; }

    [Required]
    public int[] AuthorIds { get; set; } = [];

    [Required]
    public int[] GenreIds { get; set; } = [];
}