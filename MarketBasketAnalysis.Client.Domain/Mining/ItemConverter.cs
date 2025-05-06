using System;
using System.Collections.Generic;
using System.Linq;
using static MarketBasketAnalysis.Client.Domain.Mining.ItemsetConversionResult;

namespace MarketBasketAnalysis.Client.Domain.Mining
{
    /// <inheritdoc />
    public sealed class ItemConverter : IItemConverter
    {
        #region Fields and Properties

        private readonly Dictionary<Item, ItemConversionRule> _replacementRules;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemConverter"/> class with the specified collection of conversion rules.
        /// </summary>
        /// <param name="conversionRules">
        /// A collection of <see cref="ItemConversionRule"/> objects that define the rules for converting items.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="conversionRules"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="conversionRules"/> contains <c>null</c> elements or duplicate rules.
        /// </exception>
        public ItemConverter(IReadOnlyCollection<ItemConversionRule> conversionRules)
        {
            if (conversionRules == null)
                throw new ArgumentNullException(nameof(conversionRules));

            if (conversionRules.Any(item => item == null))
            {
                throw new ArgumentException("Collection of item conversion rules cannot contain null items.",
                    nameof(conversionRules));
            }

            if (conversionRules.Distinct().Count() != conversionRules.Count)
            {
                throw new ArgumentException("Collection of item conversion rules cannot contain same items.",
                    nameof(conversionRules));
            }

            _replacementRules = conversionRules.ToDictionary(rule => rule.SourceItem);
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public ItemsetConversionResult TryConvert(Item item1, Item item2,
            out Item convertedItem1, out Item convertedItem2)
        {
            if (item1 == null)
                throw new ArgumentNullException(nameof(item1));
            
            if (item2 == null)
                throw new ArgumentNullException(nameof(item2));
            
            if (item1.Equals(item2))
                throw new ArgumentException("Converted items must not be the same.");

            _replacementRules.TryGetValue(item1, out var replacementRule1);
            _replacementRules.TryGetValue(item2, out var replacementRule2);

            if (replacementRule1 == null && replacementRule2 == null)
            {
                convertedItem1 = default;
                convertedItem2 = default;

                return NoConversionRequired;
            }

            convertedItem1 = replacementRule1?.TargetItem ?? item1;
            convertedItem2 = replacementRule2?.TargetItem ?? item2;

            if (convertedItem1.Equals(convertedItem2))
                return ConvertedItemsetHasSameItems;

            return ItemsetConverted;
        }

        /// <inheritdoc />
        public bool TryGetGroupItem(Item item, out Item groupItem)
        {
            if (_replacementRules.TryGetValue(item, out var replacementRule))
            {
                groupItem = replacementRule.TargetItem;

                return true;
            }

            groupItem = default;

            return false;
        }

        #endregion
    }
}