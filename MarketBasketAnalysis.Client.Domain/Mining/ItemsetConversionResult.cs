namespace MarketBasketAnalysis.Client.Domain.Mining
{
    /// <summary>
    /// Represents the result of attempting to convert a pair of items during association rule mining.
    /// </summary>
    public enum ItemsetConversionResult
    {
        /// <summary>
        /// Indicates that the itemset was successfully converted into group representations.
        /// </summary>
        ItemsetConverted,

        /// <summary>
        /// Indicates that no conversion was performed.
        /// </summary>
        NoConversionRequired,

        /// <summary>
        /// Indicates that the converted itemset contains identical items.
        /// </summary>
        ConvertedItemsetHasSameItems
    }
}
