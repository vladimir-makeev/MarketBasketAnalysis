using System;

namespace MarketBasketAnalysis.Client.Domain.Mining
{
    /// <summary>
    /// Provides methods for converting itemsets of two items into their group representations 
    /// and retrieving group representations for individual items during association rule mining.
    /// </summary>
    public interface IItemConverter
    {
        /// <summary>
        /// Attempts to convert a pair of items into their corresponding group representations.
        /// </summary>
        /// <param name="item1">The first item to be converted.</param>
        /// <param name="item2">The second item to be converted.</param>
        /// <param name="convertedItem1">
        /// When this method returns, contains the converted form of <paramref name="item1"/>, 
        /// if a conversion is possible; otherwise, contains the original <paramref name="item1"/>.
        /// </param>
        /// <param name="convertedItem2">
        /// When this method returns, contains the converted form of <paramref name="item2"/>, 
        /// if a conversion is possible; otherwise, contains the original <paramref name="item2"/>.
        /// </param>
        /// <returns>
        /// An <see cref="ItemsetConversionResult"/> value indicating the result of the conversion:
        /// <list type="bullet">
        /// <item><see cref="ItemsetConversionResult.ItemsetConverted"/> if the items were successfully converted into groups.</item>
        /// <item><see cref="ItemsetConversionResult.NoConversionRequired"/> if no conversion was performed.</item>
        /// <item><see cref="ItemsetConversionResult.ConvertedItemsetHasSameItems"/> if the converted items are identical.</item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="item1"/> or <paramref name="item2"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="item1"/> and <paramref name="item2"/> are the same.</exception>
        ItemsetConversionResult TryConvert(Item item1, Item item2, out Item convertedItem1, out Item convertedItem2);

        /// <summary>
        /// Attempts to retrieve the group item associated with the specified item.
        /// </summary>
        /// <param name="item">The item for which to retrieve the group item.</param>
        /// <param name="groupItem">
        /// When this method returns, contains the group item associated with <paramref name="item"/>, 
        /// if a conversion rule exists; otherwise, contains <c>null</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if a group item was successfully retrieved; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is <c>null</c>.</exception>
        bool TryGetGroupItem(Item item, out Item groupItem);
    }
}
