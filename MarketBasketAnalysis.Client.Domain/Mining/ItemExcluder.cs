using ConcurrentCollections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketBasketAnalysis.Client.Domain.Mining
{
    /// <inheritdoc />
    public class ItemExcluder : IItemExcluder
    {
        #region Fields and Properties

        private readonly IReadOnlyCollection<ItemExclusionRule> _exclusionRules;

        private readonly ConcurrentHashSet<Item> _allowedItems;
        private readonly ConcurrentHashSet<Item> _notAllowedItems;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemExcluder"/> class with the specified collection of exclusion rules.
        /// </summary>
        /// <param name="exclusionRules">
        /// A collection of <see cref="ItemExclusionRule"/> objects that define the rules for excluding items.
        /// Each rule specifies the criteria for determining whether an item should be excluded.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="exclusionRules"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="exclusionRules"/> contains <c>null</c> elements.
        /// </exception>
        public ItemExcluder(IReadOnlyCollection<ItemExclusionRule> exclusionRules)
        {
            if (exclusionRules == null)
                throw new ArgumentNullException(nameof(exclusionRules));

            if (exclusionRules.Any(item => item == null))
            {
                throw new ArgumentException("Collection of item exclusion rules cannot contain null items.",
                    nameof(exclusionRules));
            }

            _exclusionRules = exclusionRules;

            _allowedItems = new ConcurrentHashSet<Item>();
            _notAllowedItems = new ConcurrentHashSet<Item>();
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public bool ShouldExclude(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (_allowedItems.Contains(item))
                return false;

            if (_notAllowedItems.Contains(item))
                return true;

            var shouldExclude = false;

            foreach (var exclusionRule in _exclusionRules)
            {
                if (exclusionRule.ShouldExclude(item))
                {
                    shouldExclude = true;

                    break;
                }
            }

            if (shouldExclude)
                _notAllowedItems.Add(item);
            else
                _allowedItems.Add(item);

            return shouldExclude;
        }

        #endregion
    }
}
