using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarketBasketAnalysis.Client.Domain.Mining
{
    /// <summary>
    /// Defines an interface for performing association rule mining based on transaction data.
    /// </summary>
    public interface IMiner
    {
        /// <summary>
        /// Event triggered when the mining progress changes.
        /// </summary>
        event EventHandler<double> MiningProgressChanged;

        /// <summary>
        /// Event triggered when the mining stage changes.
        /// </summary>
        event EventHandler<MiningStage> MiningStageChanged;

        /// <summary>
        /// Performs association rule mining synchronously.
        /// </summary>
        /// <param name="transactions">A collection of transactions, where each transaction is represented as an array of items.</param>
        /// <param name="parameters">The mining parameters, including minimum support and confidence thresholds.</param>
        /// <returns>A collection of association rules that meet the specified parameters.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="transactions"/> or <paramref name="parameters"/> is <c>null</c>.</exception>
        IReadOnlyCollection<AssociationRule> Mine(IEnumerable<Item[]> transactions, MiningParameters parameters);

        /// <summary>
        /// Performs association rule mining asynchronously.
        /// </summary>
        /// <param name="transactions">A collection of transactions, where each transaction is represented as an array of items.</param>
        /// <param name="parameters">The mining parameters, including minimum support and confidence thresholds.</param>
        /// <param name="token">A cancellation token to interrupt the operation.</param>
        /// <returns>A task representing the asynchronous operation, with a result of a collection of association rules.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="transactions"/> or <paramref name="parameters"/> is <c>null</c>.</exception>
        /// <exception cref="OperationCanceledException">
        /// Thrown if the operation is canceled via the <paramref name="token"/>.
        /// </exception>
        Task<IReadOnlyCollection<AssociationRule>> MineAsync(IEnumerable<Item[]> transactions, MiningParameters parameters, CancellationToken token = default);
    }
}
