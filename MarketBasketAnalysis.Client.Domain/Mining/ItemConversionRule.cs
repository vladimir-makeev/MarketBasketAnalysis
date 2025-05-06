using System;

namespace MarketBasketAnalysis.Client.Domain.Mining
{
    /// <summary>
    /// Represents a rule for converting one item into another.
    /// </summary>
    public sealed class ItemConversionRule : IEquatable<ItemConversionRule>
    {
        #region Fields and Properties

        /// <summary>
        /// Gets the source item that will be converted.
        /// </summary>
        public Item SourceItem { get; }

        /// <summary>
        /// Gets the target item to which the source item will be converted.
        /// </summary>
        public Item TargetItem { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemConversionRule"/> class with the specified source and target items.
        /// </summary>
        /// <param name="sourceItem">The source <see cref="Item"/> to be converted.</param>
        /// <param name="targetItem">The target <see cref="Item"/> to which the source item will be converted.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="sourceItem"/> or <paramref name="targetItem"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="sourceItem"/> is a group or if <paramref name="targetItem"/> is not a group.
        /// </exception>
        public ItemConversionRule(Item sourceItem, Item targetItem)
        {
            if (sourceItem == null)
                throw new ArgumentNullException(nameof(sourceItem));

            if (sourceItem.IsGroup)
            {
                throw new ArgumentException(
                    "Source item cannot be a group because it represents transaction data item.",
                    nameof(sourceItem));
            }

            if (targetItem == null)
                throw new ArgumentNullException(nameof(targetItem));

            if (!targetItem.IsGroup)
                throw new ArgumentException("Target item must be group.", nameof(targetItem));

            SourceItem = sourceItem;
            TargetItem = targetItem;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified <see cref="ItemConversionRule"/> is equal to the current instance.
        /// </summary>
        /// <param name="other">The <see cref="ItemConversionRule"/> to compare with the current instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="ItemConversionRule"/> is equal to the current instance; otherwise, <c>false</c>.
        /// </returns>
        bool IEquatable<ItemConversionRule>.Equals(ItemConversionRule other) =>
            EqualsInternal(other);

        private bool EqualsInternal(ItemConversionRule other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return SourceItem.Equals(other.SourceItem) && TargetItem.Equals(other.TargetItem);
        }

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            EqualsInternal(obj as ItemConversionRule);

        /// <inheritdoc />
        public override int GetHashCode() =>
            SourceItem.GetHashCode() * 397 ^ TargetItem.GetHashCode();

        /// <inheritdoc />
        public override string ToString() =>
            $"{SourceItem.Name} -> {TargetItem.Name}";

        #endregion 
    }
}
