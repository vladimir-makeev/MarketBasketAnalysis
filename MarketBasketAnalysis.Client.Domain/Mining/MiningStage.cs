namespace MarketBasketAnalysis.Client.Domain.Mining
{
    /// <summary>
    /// Represents the stages of the association rule mining process.
    /// </summary>
    public enum MiningStage
    {
        /// <summary>
        /// The stage where frequent items are identified based on the minimum support threshold.
        /// </summary>
        FrequentItemSearch,

        /// <summary>
        /// The stage where itemsets are generated from the frequent items.
        /// </summary>
        ItemsetSearch,

        /// <summary>
        /// The stage where association rules are generated from the itemsets.
        /// </summary>
        AssociationRuleGeneration
    }
}