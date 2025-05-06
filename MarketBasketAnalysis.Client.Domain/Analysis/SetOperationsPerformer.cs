using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketBasketAnalysis.Client.Domain.Analysis
{
    /// <inheritdoc />
    public sealed class SetOperationsPerformer : ISetOperationsPerformer
    {
        #region Nested types

        private sealed class AssociationRuleUndirectedEqualityComparer : IEqualityComparer<AssociationRule>
        {
            public bool Equals(AssociationRule x, AssociationRule y)
            {
                if (ReferenceEquals(x, y))
                    return true;

                if (x == null || y == null)
                    return false;

                return x.Equals(y) || (x.LeftHandSide.Id == y.RightHandSide.Id && x.RightHandSide.Id == y.LeftHandSide.Id);
            }

            public int GetHashCode(AssociationRule obj)
            {
                if (obj == null)
                    throw new ArgumentNullException(nameof(obj));

                return obj.LeftHandSide.Id ^ obj.RightHandSide.Id;
            }
        }

        #endregion

        #region Fields and Properties

        private readonly IEqualityComparer<AssociationRule> _directedEqualityComparer =
            EqualityComparer<AssociationRule>.Default;
        private readonly IEqualityComparer<AssociationRule> _undirectedEqualityComparer =
            new AssociationRuleUndirectedEqualityComparer();

        #endregion

        #region Methods

        /// <inheritdoc />
        public IReadOnlyCollection<AssociationRule> Except(IReadOnlyCollection<AssociationRule> first,
            IReadOnlyCollection<AssociationRule> second, bool ignoreLinkDirection = false)
        {
            ValidateAssociationRules(first, nameof(first));
            ValidateAssociationRules(second, nameof(second));

            return first
                .Except(second, GetEqualityComparer(ignoreLinkDirection))
                .ToList();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<AssociationRule> Intersect(IReadOnlyCollection<AssociationRule> first,
            IReadOnlyCollection<AssociationRule> second, bool ignoreLinkDirection = false)
        {
            ValidateAssociationRules(first, nameof(first));
            ValidateAssociationRules(second, nameof(second));

            return first
                .Intersect(second, GetEqualityComparer(ignoreLinkDirection))
                .ToList();
        }

        private static void ValidateAssociationRules(IReadOnlyCollection<AssociationRule> associationRules, string paramName)
        {
            if (associationRules == null)
                throw new ArgumentNullException(paramName);

            if (associationRules.Any(item => item == null))
                throw new ArgumentException("Collection of association rules cannot contain null items.", paramName);
        }

        private IEqualityComparer<AssociationRule> GetEqualityComparer(bool ignoreLinkDirection) =>
            ignoreLinkDirection ? _undirectedEqualityComparer : _directedEqualityComparer;

        #endregion
    }
}