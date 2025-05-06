using System;

namespace MarketBasketAnalysis.Client.Domain
{
    /// <summary>
    /// Represents an item in a transaction.
    /// </summary>
    public sealed class Item : IEquatable<Item>
    {
        #region Fields and Properties

        /// <summary>
        /// Gets the unique identifier of the item.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the item is a group of other items.
        /// </summary>
        public bool IsGroup { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Item"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the item.</param>
        /// <param name="name">The name of the item.</param>
        /// <param name="isGroup">A value indicating whether the item is a group of other items.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/> is <c>null</c>.</exception>
        public Item(int id, string name, bool isGroup)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            Id = id;
            IsGroup = isGroup;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        bool IEquatable<Item>.Equals(Item other) =>
            EqualsInternal(other);

        /// <inheritdoc />
        public override bool Equals(object obj) =>
            EqualsInternal(obj as Item);

        private bool EqualsInternal(Item other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        /// <inheritdoc />
        public override int GetHashCode() =>
            Id.GetHashCode();

        /// <inheritdoc />
        public override string ToString() => Name;

        #endregion
    }
}