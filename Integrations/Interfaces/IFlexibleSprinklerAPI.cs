﻿using Microsoft.Xna.Framework;

namespace AtraShared.Integrations.Interfaces;

#pragma warning disable SA1623 // Property summary documentation should match accessors - preserving original comments.
#pragma warning disable SA1611 // Element parameters should be documented
#pragma warning disable SA1615 // Element return value should be documented

/// <summary>The API which provides access to Flexible Sprinklers for other mods.</summary>
/// <remarks>Copied from: https://github.com/Shockah/Stardew-Valley-Mods/blob/master/FlexibleSprinklers/IFlexibleSprinklersApi.cs. </remarks>
public interface IFlexibleSprinklersApi
{
    /// <summary>Returns whether the current configuration allows independent sprinkler activation.</summary>
    bool IsSprinklerBehaviorIndependent { get; }

    /// <summary>
    /// Register a new sprinkler tier provider, to add support for Flexible Sprinklers for your custom tiered sprinklers in your mod or override existing ones.<br/>
    /// This is only used for tiered sprinkler power config overrides (how many tiles they water).<br/>
    /// Return `null` if you don't want to modify this specific tier.
    /// </summary>
    void RegisterSprinklerTierProvider(Func<SObject, int?> provider);

    /// <summary>
    /// Register a new sprinkler coverage provider, to add support for Flexible Sprinklers for your custom sprinklers in your mod or override existing ones.<br/>
    /// Returned tile coverage should be relative.<br />
    /// Return `null` if you don't want to modify this specific coverage.
    /// </summary>
    void RegisterSprinklerCoverageProvider(Func<SObject, Vector2[]> provider);

    /// <summary>
    /// Registers a new custom waterable tile provider, to make some tiles count as waterable or not.<br/>
    /// Return `true` if the tile should be waterable no matter what; return `false` if the tile should not be waterable no matter what; return `null` if you don't want to modify this specific tile.
    /// </summary>
    void RegisterCustomWaterableTileProvider(Func<GameLocation, Vector2, bool?> provider);

    /// <summary>Activates all sprinklers in a collective way, taking into account the Flexible Sprinklers mod behavior.</summary>
    void ActivateAllSprinklers();

    /// <summary>Activates sprinklers in specified location in a collective way, taking into account the Flexible Sprinklers mod behavior.</summary>
    void ActivateSprinklersInLocation(GameLocation location);

    /// <summary>Activates a sprinkler, taking into account the Flexible Sprinklers mod behavior.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the current sprinkler behavior does not allow independent sprinkler activation.</exception>
    void ActivateSprinkler(SObject sprinkler, GameLocation location);

    /// <summary>Returns the sprinkler's power after config modifications (that is, the number of tiles it will water).</summary>
    int GetSprinklerPower(SObject sprinkler);

    /// <summary>Returns a sprinkler's flood fill range (that is, how many tiles away will it look for tiles to water) for a given sprinkler power.</summary>
    int GetFloodFillSprinklerRange(int power);

    /// <summary>Get the relative tile coverage by supported sprinkler ID. This API is location/position-agnostic. Note that sprinkler IDs may change after a save is loaded due to Json Assets reallocating IDs.</summary>
    Vector2[] GetUnmodifiedSprinklerCoverage(SObject sprinkler);

    /// <summary>Get the relative tile coverage by supported sprinkler ID, modified by the Flexible Sprinklers mod. This API takes into consideration the location and position. Note that sprinkler IDs may change after a save is loaded due to Json Assets reallocating IDs.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the current sprinkler behavior does not allow independent sprinkler activation.</exception>
    Vector2[] GetModifiedSprinklerCoverage(SObject sprinkler, GameLocation location);

    /// <summary>Returns whether a given tile is in range of any sprinkler in the location.</summary>
    bool IsTileInRangeOfAnySprinkler(GameLocation location, Vector2 tileLocation);

    /// <summary>Returns whether a given tile is in range of the specified sprinkler.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the current sprinkler behavior does not allow independent sprinkler activation.</exception>
    bool IsTileInRangeOfSprinkler(SObject sprinkler, GameLocation location, Vector2 tileLocation);

    /// <summary>Returns whether a given tile is in range of specified sprinklers.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the current sprinkler behavior does not allow independent sprinkler activation.</exception>
    bool IsTileInRangeOfSprinklers(IEnumerable<SObject> sprinklers, GameLocation location, Vector2 tileLocation);

    /// <summary>Displays the sprinkler coverage for the specified time.</summary>
    /// <param name="seconds">The amount of seconds to display the coverage for. Pass `null` to use the value configured by the user.</param>
    void DisplaySprinklerCoverage(float? seconds = null);

#pragma warning restore SA1615 // Element return value should be documented
#pragma warning restore SA1611 // Element parameters should be documented
#pragma warning restore SA1623 // Property summary documentation should match accessors
}