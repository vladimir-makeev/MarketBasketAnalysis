using System;

namespace MarketBasketAnalysis.Client.Domain.Mining
{
    /// <summary>
    /// Defines a method to determine whether a specific item should be excluded during association rule mining.
    /// </summary>
    public interface IItemExcluder
    {
        /// <summary>
        /// Determines whether the specified item should be excluded.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the item should be excluded; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is <c>null</c>.</exception>
        bool ShouldExclude(Item item);
    }
}