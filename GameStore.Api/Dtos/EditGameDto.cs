using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class EditGameDto
(
    [Required][StringLength(50)] string Name,
    int GenreId,
    [Range(1, 100)] decimal Price,
    DateOnly ReleaseDate
);