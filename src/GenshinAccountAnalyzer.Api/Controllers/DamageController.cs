using GenshinAccountAnalyzer.Application.Abstractions;
using GenshinAccountAnalyzer.Domain.Damage;
using Microsoft.AspNetCore.Mvc;

namespace GenshinAccountAnalyzer.Api.Controllers;

/// <summary>
/// Endpoints exposing the damage calculator's game formulas.
/// </summary>
[ApiController]
[Route("api/damage")]
public sealed class DamageController : ControllerBase
{
    private readonly IDamageCalculator _calculator;

    /// <summary>Initializes the controller.</summary>
    /// <param name="calculator">The damage calculator.</param>
    public DamageController(IDamageCalculator calculator)
    {
        _calculator = calculator;
    }

    /// <summary>Computes the non-crit, crit and averaged damage of a single hit.</summary>
    /// <param name="input">The resolved hit parameters (all buffs already folded in).</param>
    /// <returns>The <see cref="DamageResult"/>.</returns>
    /// <response code="200">The damage was computed.</response>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(DamageResult), StatusCodes.Status200OK)]
    public ActionResult<DamageResult> Calculate([FromBody] DamageInput input) =>
        Ok(_calculator.CalculateHit(input));

    /// <summary>Computes transformative reaction damage.</summary>
    /// <param name="request">The transformative reaction parameters.</param>
    /// <returns>The reaction damage.</returns>
    /// <response code="200">The reaction damage was computed.</response>
    [HttpPost("transformative")]
    [ProducesResponseType(typeof(TransformativeResponse), StatusCodes.Status200OK)]
    public ActionResult<TransformativeResponse> Transformative([FromBody] TransformativeRequest request)
    {
        double damage = _calculator.CalculateTransformative(
            request.Reaction,
            request.CharacterLevel,
            request.ElementalMastery,
            request.ReactionBonus,
            request.Enemy ?? EnemyProfile.Standard90);

        return Ok(new TransformativeResponse(damage));
    }

    /// <summary>Request body for the transformative reaction endpoint.</summary>
    /// <param name="Reaction">The transformative reaction.</param>
    /// <param name="CharacterLevel">The reacting character's level.</param>
    /// <param name="ElementalMastery">The reacting character's Elemental Mastery.</param>
    /// <param name="ReactionBonus">Extra reaction bonus (fraction).</param>
    /// <param name="Enemy">The target profile; a level-90 default is used when omitted.</param>
    public sealed record TransformativeRequest(
        TransformativeReaction Reaction,
        int CharacterLevel,
        double ElementalMastery,
        double ReactionBonus,
        EnemyProfile? Enemy);

    /// <summary>Response for the transformative reaction endpoint.</summary>
    /// <param name="Damage">The computed reaction damage.</param>
    public sealed record TransformativeResponse(double Damage);
}
