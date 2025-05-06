using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static MarketBasketAnalysis.Client.Domain.Mining.ItemsetConversionResult;
using Timer = System.Timers.Timer;

namespace MarketBasketAnalysis.Client.Domain.Mining
{
    /// <inheritdoc />
    public sealed class Miner : IMiner
    {
        #region Fields and Properties

        /// <inheritdoc />
        public event EventHandler<double> MiningProgressChanged;

        /// <inheritdoc />
        public event EventHandler<MiningStage> MiningStageChanged;

        #endregion

        #region Methods

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public Task<IReadOnlyCollection<AssociationRule>> MineAsync(IEnumerable<Item[]> transactions,
            MiningParameters parameters, CancellationToken token = default)
        {
            ValidateParameters(transactions, parameters);

            return Task.Run((() => MineInternal(transactions, parameters, token)), token);
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public IReadOnlyCollection<AssociationRule> Mine(IEnumerable<Item[]> transactions,
            MiningParameters parameters)
        {
            ValidateParameters(transactions, parameters);

            return MineInternal(transactions, parameters, default);
        }

        private static void ValidateParameters(IEnumerable<Item[]> transactions, MiningParameters parameters)
        {
            if (transactions == null)
                throw new ArgumentNullException(nameof(transactions));

            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
        }

        private IReadOnlyCollection<AssociationRule> MineInternal(IEnumerable<Item[]> transactions,
            MiningParameters parameters, CancellationToken token)
        {
            MiningStageChanged?.Invoke(this, MiningStage.FrequentItemSearch);

            // ReSharper disable once PossibleMultipleEnumeration
            var frequentItems = SearchForFrequentItems(transactions, parameters, token, out var transactionCount);

            MiningStageChanged?.Invoke(this, MiningStage.ItemsetSearch);

            // ReSharper disable once PossibleMultipleEnumeration
            var itemsets = SearchForItemsets(transactions, parameters, frequentItems, transactionCount,
                progress => MiningProgressChanged?.Invoke(this, progress), token);

            MiningStageChanged?.Invoke(this, MiningStage.AssociationRuleGeneration);

            return GenerateAssociationRules(itemsets, frequentItems, transactionCount, parameters, token);
        }

        private static Dictionary<Item, int> SearchForFrequentItems(IEnumerable<Item[]> transactions,
            MiningParameters parameters, CancellationToken token, out int transactionCount)
        {
            var transactionCountInternal = 0;

            var itemFrequencies = new ConcurrentDictionary<Item, int>(parameters.DegreeOfParallelism, 0);

            transactions
                .AsParallel()
                .WithDegreeOfParallelism(parameters.DegreeOfParallelism)
                .WithCancellation(token)
                .ForAll(transaction =>
                {
                    ThrowIfTransactionIsNull(transaction);

                    foreach (var item in transaction)
                    {
                        if (parameters.ItemExcluder?.ShouldExclude(item) ?? false)
                            continue;

                        var resultItem = item;

                        if (parameters.ItemConverter?.TryGetGroupItem(item, out var groupItem) ?? false)
                        {
                            if (parameters.ItemExcluder?.ShouldExclude(groupItem) ?? false)
                                continue;

                            resultItem = groupItem;
                        }

                        itemFrequencies.AddOrUpdate(resultItem, 1, (_, itemFrequency) => itemFrequency + 1);
                    }

                    Interlocked.Increment(ref transactionCountInternal);
                });

            transactionCount = transactionCountInternal;

            var frequencyThreshold = (int)Math.Ceiling(transactionCount * parameters.MinSupport);

            return itemFrequencies
                .Where(keyValuePair => keyValuePair.Value >= frequencyThreshold)
                .ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value);
        }

        private static ConcurrentDictionary<(Item, Item), int> SearchForItemsets(IEnumerable<Item[]> transactions,
            MiningParameters parameters, Dictionary<Item, int> frequentItems, int transactionCount,
            Action<double> fireMiningProgressChangedEvent, CancellationToken token)
        {
            var previousProcessedTransactionsCount = 0;
            var processedTransactionCount = 0;

            // ToDo: calculate progress value more accurately
            var timer = new Timer(100);

            timer.Elapsed += Timer_Elapsed;

            timer.Start();

            var itemsetFrequencies = new ConcurrentDictionary<(Item, Item), int>(parameters.DegreeOfParallelism, 0);

            try
            {
                transactions
                    .AsParallel()
                    .WithCancellation(token)
                    .WithDegreeOfParallelism(parameters.DegreeOfParallelism)
                    .ForAll(transaction =>
                    {
                        ThrowIfTransactionIsNull(transaction);
                        
                        for (var i = 0; i < transaction.Length; i++)
                            for (var j = i + 1; j < transaction.Length; j++)
                            {
                                var (resultItem1, resultItem2) = transaction[i].Id < transaction[j].Id
                                        ? (transaction[i], transaction[j])
                                        : (transaction[j], transaction[i]);

                                if (parameters.ItemConverter != null)
                                {
                                    var conversionResult = parameters.ItemConverter.TryConvert(resultItem1, resultItem2,
                                        out var convertedItem1, out var convertedItem2);

                                    if (conversionResult == ConvertedItemsetHasSameItems)
                                        continue;

                                    resultItem1 = convertedItem1;
                                    resultItem2 = convertedItem2;
                                }

                                if (frequentItems.ContainsKey(resultItem1) && frequentItems.ContainsKey(resultItem2))
                                {
                                    itemsetFrequencies.AddOrUpdate((resultItem1, resultItem2), 1,
                                        (_, itemsetFrequency) => itemsetFrequency + 1);
                                }
                            }

                        processedTransactionCount++;
                    });
            }
            finally
            {
                timer.Elapsed -= Timer_Elapsed;
                timer.Dispose();
            }

            return itemsetFrequencies;

            // ReSharper disable once InconsistentNaming
            void Timer_Elapsed(object sender, ElapsedEventArgs e)
            {
                if (processedTransactionCount == previousProcessedTransactionsCount)
                    return;

                var progress = processedTransactionCount / (double)transactionCount * 100;

                previousProcessedTransactionsCount = processedTransactionCount;

                fireMiningProgressChangedEvent(progress);
            }
        }

        private static void ThrowIfTransactionIsNull(Item[] transaction)
        {
            if (transaction == null)
                throw new InvalidOperationException("Transaction cannot be null.");
        }

        private ConcurrentBag<AssociationRule> GenerateAssociationRules(ConcurrentDictionary<(Item, Item), int> frequentItemsets,
            Dictionary<Item, int> frequentItems, int transactionCount, MiningParameters parameters, CancellationToken token)
        {
            var frequencyThreshold = (int)Math.Ceiling(transactionCount * parameters.MinSupport);
            var associationRules = new ConcurrentBag<AssociationRule>();

            frequentItemsets
                .AsParallel()
                .WithDegreeOfParallelism(parameters.DegreeOfParallelism)
                .WithCancellation(token)
                .ForAll(GenerateAssociationRulePair);

            void GenerateAssociationRulePair(KeyValuePair<(Item, Item), int> keyValuePair)
            {
                var itemsetFrequency = keyValuePair.Value;

                if (itemsetFrequency < frequencyThreshold)
                    return;

                var (item1, item2) = keyValuePair.Key;
                var itemFrequency1 = frequentItems[item1];
                var itemFrequency2 = frequentItems[item2];

                if (itemsetFrequency / (double)itemFrequency1 >= parameters.MinConfidence)
                {
                    associationRules.Add(new AssociationRule(item1, item2, itemFrequency1, itemFrequency2,
                        itemsetFrequency, transactionCount));
                }

                if (itemsetFrequency / (double)itemFrequency2 >= parameters.MinConfidence)
                {
                    associationRules.Add(new AssociationRule(item2, item1, itemFrequency2, itemFrequency1,
                        itemsetFrequency, transactionCount));
                }
            }

            return associationRules;
        }

        #endregion
    }
}