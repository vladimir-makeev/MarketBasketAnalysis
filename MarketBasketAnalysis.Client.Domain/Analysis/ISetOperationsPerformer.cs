using System;
using System.Collections.Generic;

namespace MarketBasketAnalysis.Client.Domain.Analysis
{
    /// <summary>
    /// Defines an interface for performing set operations on collections of association rules.
    /// </summary>
    public interface ISetOperationsPerformer
    {
        /// <summary>
        /// Computes the intersection of two collections of association rules.
        /// </summary>
        /// <param name="first">The first collection of association rules.</param>
        /// <param name="second">The second collection of association rules.</param>
        /// <param name="ignoreLinkDirection">
        /// A value indicating whether the direction of links between association rules should be ignored.
        /// If <c>true</c>, the intersection will consider rules as equal regardless of their direction.
        /// </param>
        /// <returns>
        /// A collection of association rules that are present in both <paramref name="first"/> and <paramref name="second"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="first"/> or <paramref name="second"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="first"/> or <paramref name="second"/> contains <c>null</c> items.
        /// </exception>
        IReadOnlyCollection<AssociationRule> Intersect(IReadOnlyCollection<AssociationRule> first,
            IReadOnlyCollection<AssociationRule> second, bool ignoreLinkDirection = false);

        /// <summary>
        /// Computes the difference between two collections of association rules.
        /// </summary>
        /// <param name="first">The first collection of association rules.</param>
        /// <param name="second">The second collection of association rules.</param>
        /// <param name="ignoreLinkDirection">
        /// A value indicating whether the direction of links between association rules should be ignored.
        /// If <c>true</c>, the difference will consider rules as equal regardless of their direction.
        /// </param>
        /// <returns>
        /// A collection of association rules that are present in <paramref name="first"/> but not in <paramref name="second"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="first"/> or <paramref name="second"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="first"/> or <paramref name="second"/> contains <c>null</c> items.
        /// </exception>
        IReadOnlyCollection<AssociationRule> Except(IReadOnlyCollection<AssociationRule> first,
            IReadOnlyCollection<AssociationRule> second, bool ignoreLinkDirection = false);
    }
}