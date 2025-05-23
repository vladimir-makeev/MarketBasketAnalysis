﻿using MarketBasketAnalysis.Common.Protos;
using MarketBasketAnalysis.Server.Application.Exceptions;
using MarketBasketAnalysis.Server.Application.Extensions;
using MarketBasketAnalysis.Server.Data;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Runtime.CompilerServices;

namespace MarketBasketAnalysis.Server.Application.Services;

public sealed class AssociationRuleSetLoader : IAssociationRuleSetLoader
{
    #region Fields and properties

    private readonly MarketBasketAnalysisDbContext _context;

    #endregion

    #region Constructors

    public AssociationRuleSetLoader(MarketBasketAnalysisDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        _context = context;
    }

    #endregion

    #region Methods

    public async Task<AssociationRuleSetInfoMessage> LoadAssociationRuleSetInfoAsync(
        string associationRuleSetName, CancellationToken token = default)
    {
        associationRuleSetName.ValidateAssociationRuleSetName();

        var associationRuleSet = await _context.AssociationRuleSets
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Name == associationRuleSetName, token);

        if (associationRuleSet == null)
            throw new AssociationRuleSetNotFoundException(associationRuleSetName);

        return new AssociationRuleSetInfoMessage
        {
            Name = associationRuleSet.Name,
            Description = associationRuleSet.Description,
            TransactionCount = associationRuleSet.TransactionCount
        };
    }

    public IAsyncEnumerable<ItemChunkMessage> LoadItemChunksAsync(string associationRuleSetName,
        CancellationToken token = default)
    {
        associationRuleSetName.ValidateAssociationRuleSetName();

        return AsyncEnumerableEx
            .Defer(() => LoadItemsChunkInternalAsync(associationRuleSetName, token))
            .Catch<ItemChunkMessage, DbException>((e, _) =>
                throw new AssociationRuleSetLoadException(
                    "Unexpected error occurred while loading item chunks.", e));
    }

    private async IAsyncEnumerable<ItemChunkMessage> LoadItemsChunkInternalAsync(
        string associationRuleSetName,
        [EnumeratorCancellation] CancellationToken token)
    {
        var itemChunks = _context.ItemChunks
            .AsNoTracking()
            .Where(e => e.AssociationRuleSet!.Name == associationRuleSetName)
            .AsAsyncEnumerable()
            .WithCancellation(token);

        await foreach (var itemChunk in itemChunks)
        {
            var itemChunkMessage = ItemChunkMessage.Parser.ParseFrom(
                itemChunk.Data, 0, itemChunk.PayloadSize);

            yield return itemChunkMessage;
        }
    }

    public IAsyncEnumerable<AssociationRuleChunkMessage> LoadAssociationRuleChunksAsync(
        string associationRuleSetName, CancellationToken token = default)
    {
        associationRuleSetName.ValidateAssociationRuleSetName();

        return AsyncEnumerableEx
            .Defer(() => LoadAssociationRuleChunksInternalAsync(associationRuleSetName, token))
            .Catch<AssociationRuleChunkMessage, DbException>((e, _) =>
                throw new AssociationRuleSetLoadException(
                    "Unexpected error occurred while loading association rule chunks.", e));
    }

    private async IAsyncEnumerable<AssociationRuleChunkMessage> LoadAssociationRuleChunksInternalAsync(
        string associationRuleSetName, [EnumeratorCancellation] CancellationToken token)
    {
        var associationRuleChunks = _context.AssociationRuleChunks
            .AsNoTracking()
            .Where(e => e.AssociationRuleSet!.Name == associationRuleSetName)
            .AsAsyncEnumerable()
            .WithCancellation(token);

        await foreach (var associationRuleChunk in associationRuleChunks)
        {
            var itemChunkMessage = AssociationRuleChunkMessage.Parser.ParseFrom(
                associationRuleChunk.Data, 0, associationRuleChunk.PayloadSize);

            yield return itemChunkMessage;
        }
    }

    #endregion
}