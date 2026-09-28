using System.ComponentModel.DataAnnotations;

namespace RazorPub.Models;

public class BlogPost
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [StringLength(160)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "O resumo é obrigatório.")]
    [StringLength(220)]
    public string Summary { get; set; } = string.Empty;

    [Required(ErrorMessage = "O conteúdo é obrigatório.")]
    public string Content { get; set; } = string.Empty;

    [StringLength(260)]
    public string? CoverImagePath { get; set; }
    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}