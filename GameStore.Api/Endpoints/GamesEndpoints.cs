using System;
using System.Data.Common;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{

    const string GetGameEndPointName = "GetGame";
    private static readonly List<GameSummaryDto> games =
    [
    new (1, "God of War", "Adventure", 65.24M, new DateOnly(2022, 9, 30)),
new (2, "Aries", "Action", 65.22M, new DateOnly(2023, 5, 25))
    ];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
        .WithParameterValidation();

        // GET /games
        group.MapGet("/", async (GameStoreContext dbContext) =>
        await dbContext.Games
            .Include(game => game.Genre)
            .Select(game => game.ToGameSummaryDto())
            .AsNoTracking()
            .ToListAsync());


        // GET /games/1
        group.MapGet("/{id}", async (int Id, GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(Id);
            return game is null ?
            Results.NotFound() : Results.Ok(game.ToGameDetailsDto());

        })
        .WithName(GetGameEndPointName);


        // POST /games/
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            return Results.CreatedAtRoute(
                GetGameEndPointName,
                new { id = game.Id },
            game.ToGameDetailsDto());

        });

        // PUT /games/2
        group.MapPut("/{id}", async (int id, EditGameDto editedGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(existingGame)
            .CurrentValues
            .SetValues(editedGame.ToEntity(id));

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        //DELETE /games/1
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games
                   .Where(game => game.Id == id)
                   .ExecuteDeleteAsync();

            return Results.NoContent();
        });
        return group;
    }   
}
