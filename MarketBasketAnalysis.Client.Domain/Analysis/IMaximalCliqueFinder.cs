using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketBasketAnalysis.Client.Domain.Analysis
{
    /// <summary>
    /// Defines an interface for finding maximal cliques in a graph of association rules.
    /// A maximal clique is a subset of association rules where every rule is connected to every other rule,
    /// and no additional rules can be added without breaking this property.
    /// </summary>
    public interface IMaximalCliqueFinder
    {
        /// <summary>
        /// Finds maximal cliques in the given collection of association rules synchronously.
        /// </summary>
        /// <param name="associationRules">The collection of association rules to analyze.</param>
        /// <param name="parameters">The parameters for finding maximal cliques.</param>
        /// <returns>A collection of maximal cliques, where each clique is represented as a collection of association rules.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="associationRules"/> or <paramref name="parameters"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="associationRules"/> contains <c>null</c> items or duplicate items.
        /// </exception>
        IReadOnlyCollection<IReadOnlyCollection<AssociationRule>> Find(
            IReadOnlyCollection<AssociationRule> associationRules, MaximalCliqueFindingParameters parameters);

        /// <summary>
        /// Finds maximal cliques in the given collection of association rules asynchronously.
        /// </summary>
        /// <param name="associationRules">The collection of association rules to analyze.</param>
        /// <param name="parameters">The parameters for finding maximal cliques.</param>
        /// <param name="token">A cancellation token to cancel the operation if needed.</param>
        /// <returns>
        /// A task representing the asynchronous operation, with a result of a collection of maximal cliques,
        /// where each clique is represented as a collection of association rules.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="associationRules"/> or <paramref name="parameters"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="associationRules"/> contains <c>null</c> items or duplicate items.
        /// </exception>
        /// <exception cref="OperationCanceledException">
        /// Thrown if the operation is canceled via the <paramref name="token"/>.
        /// </exception>
        Task<IReadOnlyCollection<IReadOnlyCollection<AssociationRule>>> FindAsync(
            IReadOnlyCollection<AssociationRule> associationRules, MaximalCliqueFindingParameters parameters,
            CancellationToken token = default);
    }
}
