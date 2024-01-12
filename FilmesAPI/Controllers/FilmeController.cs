using AutoMapper;
using FilmesApi.Data;
using FilmesApi.Data.Dtos;
using FilmesApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um filme ao banco de dados
    /// </summary>
    /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionarFilme([FromBody]CreateFilmeDto filmeDto)
    {
        var filme = _mapper.Map<Filme>(filmeDto);
        _context.Add(filme);
        _context.SaveChanges();
        Console.WriteLine(filme.Titulo);
        Console.WriteLine(filme.Duracao);
        return CreatedAtAction(nameof(RecuperaFilmePorId), new { id = filme.Id }, filme);
    }

    /// <summary>
    /// Recupera a lista de filmes do banco de dados
    /// </summary>
    /// <returns><![CDATA[ IEnumerable<ReadFilmeDto> ]]></returns>
    /// <response code="200">Retorna a lista de filmes no banco ou lista vazia caso não exista nenhum registro</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IEnumerable<ReadFilmeDto> RecuperaFilmes([FromQuery] int skip=0, [FromQuery] int take=10, [FromQuery]string? nomeCinema = null)
    {
        if(nomeCinema is null) return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take).ToList());

        return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes
            .Skip(skip).Take(take)
            .Where(f => f.Sessoes
                .Any(s => s.Cinema.Nome == nomeCinema))
            .ToList());
    }

    /// <summary>
    /// Recupera um filme do banco de dados pelo seu id
    /// </summary>
    /// <param name="id">Id de registro do filme no banco de dados</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso consulta seja feita com sucesso</response>
    /// <response code="404">Caso o id não corresponda a nenhum registro</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult RecuperaFilmePorId(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(f => f.Id == id);
        var filmeDto = _mapper.Map<ReadFilmeDto>(filme);
        if (filme is null) return NotFound();
        return Ok(filmeDto);
    }

    /// <summary>
    /// Recupera um filme do banco de dados pelo seu id e altera seus campos pelos valores do objeto fornecido
    /// </summary>
    /// <param name="id">Id de registro do filme no banco de dados</param>
    /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme, que irão sobreescrever o registro atual</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso atualização seja feita com sucesso</response>
    /// <response code="404">Caso o id não corresponda a nenhum registro</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult AtualizaFilme(int id, [FromBody]UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(f => f.Id == id);
        if (filme is null) return NotFound();
        _mapper.Map(filme, filmeDto);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Recupera um filme do banco de dados pelo seu id e altera alguns campos pelos respectivos valores fornecidos
    /// </summary>
    /// <param name="id">Id de registro do filme no banco de dados</param>
    /// <param name="patch">Objeto com os campos necessários para criação de um filme, que irão sobreescrever o registro atual</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso atualização seja feita com sucesso</response>
    /// <response code="400">Caso os valores fornecidos para atualização sejam inválidos</response>
    /// <response code="404">Caso o id não corresponda a nenhum registro</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult AtualizaFilmeParcial(int id, [FromBody] JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(f => f.Id == id);
        if (filme is null) return NotFound();

        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);
        patch.ApplyTo(filmeParaAtualizar, ModelState);

        if (!TryValidateModel(filmeParaAtualizar)) return ValidationProblem();

        _mapper.Map(filmeParaAtualizar, filme);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Apaga o registro de um filme do banco de dados pelo seu id
    /// </summary>
    /// <param name="id">Id de registro do filme no banco de dados</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso deleção seja feita com sucesso</response>
    /// <response code="404">Caso o id não corresponda a nenhum registro</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(f => f.Id == id);
        if (filme is null) return NotFound();
        _context.Remove(filme);
        _context.SaveChanges(); 
        return NoContent();
    }
}
