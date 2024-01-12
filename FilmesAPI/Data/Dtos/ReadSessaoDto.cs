using FilmesApi.Models;

namespace FilmesApi.Data.Dtos
{
    public class ReadSessaoDto
    {
        public int FilmeId { get; set; }
        public int CinemaId { get; set; }
    }
}