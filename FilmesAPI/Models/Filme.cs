using System.ComponentModel.DataAnnotations;

namespace FilmesApi.Models;

public class Filme
{
    [Key]
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "O título é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O título não pode exceder 100 caracteres.")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "O gênero é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O gênero não pode exceder 100 caracteres.")]
    public string Genero { get; set; }

    [Required(ErrorMessage = "A duração é obrigatória.")]
    [Range(1,300, ErrorMessage = "A duração deve estar entre 1 e 300 minutos.")]
    public int Duracao { get; set; }

    public virtual ICollection<Sessao> Sessoes { get; set; }
}
