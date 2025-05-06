using System;
using static System.StringComparison;

namespace MarketBasketAnalysis.Client.Domain.Mining
{
    /// <summary>
    /// Represents a rule for excluding items or groups from association rule mining.
    /// </summary>
    public class ItemExclusionRule
    {
        #region Fields and Properties

        /// <summary>
        /// Gets the pattern used to match item names for exclusion.
        /// </summary>
        public string Pattern { get; }

        /// <summary>
        /// Gets a value indicating whether the exclusion rule requires an exact match of the item name.
        /// </summary>
        public bool ExactMatch { get; }

        /// <summary>
        /// Gets a value indicating whether the comparison should ignore case when matching item names.
        /// </summary>
        public bool IgnoreCase { get; }

        /// <summary>
        /// Gets a value indicating whether the rule applies to individual items.
        /// </summary>
        public bool ApplyToItems { get; }

        /// <summary>
        /// Gets a value indicating whether the rule applies to groups of items.
        /// </summary>
        public bool ApplyToGroups { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemExclusionRule"/> class with the specified parameters.
        /// </summary>
        /// <param name="pattern">The pattern used to match item names for exclusion.</param>
        /// <param name="exactMatch">A value indicating whether the exclusion rule requires an exact match of the item name.</param>
        /// <param name="ignoreCase">A value indicating whether the comparison should ignore case when matching item names.</param>
        /// <param name="applyToItems">A value indicating whether the rule applies to individual items.</param>
        /// <param name="applyToGroups">A value indicating whether the rule applies to groups of items.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pattern"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if both <paramref name="applyToItems"/> and <paramref name="applyToGroups"/> are <c>false</c>.</exception>
        public ItemExclusionRule(string pattern, bool exactMatch, bool ignoreCase,
            bool applyToItems, bool applyToGroups)
        {
            Pattern = pattern ?? throw new ArgumentNullException(nameof(pattern));

            if (!applyToItems && !applyToGroups)
                throw new ArgumentException("Item exclusion rule must be applicable to items or groups.");

            ExactMatch = exactMatch;
            IgnoreCase = ignoreCase;
            ApplyToItems = applyToItems;
            ApplyToGroups = applyToGroups;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified item should be excluded based on the rule.
        /// </summary>
        /// <param name="item">The <see cref="Item"/> to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the item matches the exclusion rule and should be excluded; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is <c>null</c>.</exception>
        public bool ShouldExclude(Item item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!item.IsGroup && !ApplyToItems || item.IsGroup && !ApplyToGroups)
                return false;

            var comparisonType = IgnoreCase ? OrdinalIgnoreCase : Ordinal;

            return ExactMatch
                ? item.Name.Equals(Pattern, comparisonType)
                : item.Name.IndexOf(Pattern, comparisonType) != -1;
        }

        #endregion
    }
}