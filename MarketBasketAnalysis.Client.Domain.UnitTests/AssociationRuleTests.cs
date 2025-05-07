using System.Diagnostics.CodeAnalysis;

namespace MarketBasketAnalysis.Client.Domain.UnitTests;

[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
public class AssociationRuleTests
{
    private readonly Item _lhsItem = new Item(1, "Left", false);
    private readonly Item _rhsItem = new Item(2, "Right", false);
    private readonly int _lhsCount = 5;
    private readonly int _rhsCount = 5;
    private readonly int _pairCount = 3;
    private readonly int _transactionCount = 10;
    private readonly double _support = 0.3;
    private readonly double _confidence = 0.6;
    private readonly double _lift = 1.2;
    private readonly double _conviction = 1.25;
    private readonly double _yuleQCoefficient = 0.384615384615385;
    private readonly double _phiCorrelationCoefficient = 0.2;
    private readonly double _chiSquaredTestStatistic = 0.4;
    private readonly int _precision = 15;

    [Fact]
    public void Ctor_NullArguments_ThrowsArgumentNullException()
    {
        // Act
        void LeftHandSideItemIsNull() =>
            new AssociationRule(null, _rhsItem, _lhsCount, _rhsCount, _pairCount, _transactionCount);

        void RightHandSideIsNull() =>
            new AssociationRule(_lhsItem, null, _lhsCount, _rhsCount, _pairCount, _transactionCount);

        // Assert
        Assert.Throws<ArgumentNullException>(LeftHandSideItemIsNull);
        Assert.Throws<ArgumentNullException>(RightHandSideIsNull);
    }

    [Fact]
    public void Ctor_FrequencyArgumentsIsNegativeOrZero_ThrowsArgumentOutOfRangeException()
    {
        // Act
        void LhsCountIsZero() =>
            new AssociationRule(_lhsItem, _rhsItem, 0, _rhsCount, _pairCount, _transactionCount);

        void LhsCountIsNegative() =>
            new AssociationRule(_lhsItem, _rhsItem, -1, _rhsCount, _pairCount, _transactionCount);

        void RhsCountIsZero() =>
            new AssociationRule(_lhsItem, _rhsItem, _lhsCount, 0, _pairCount, _transactionCount);

        void RhsCountIsNegative() =>
            new AssociationRule(_lhsItem, _rhsItem, _lhsCount, -1, _pairCount, _transactionCount);

        void PairCountIsZero() =>
            new AssociationRule(_lhsItem, _rhsItem, _lhsCount, _rhsCount, 0, _transactionCount);

        void PairCountIsNegative() =>
            new AssociationRule(_lhsItem, _rhsItem, _lhsCount, _rhsCount, -1, _transactionCount);

        void TransactionCountIsZero() =>
            new AssociationRule(_lhsItem, _rhsItem, _lhsCount, _rhsCount, _pairCount, 0);

        void TransactionCountIsNegative() =>
            new AssociationRule(_lhsItem, _rhsItem, _lhsCount, _rhsCount, _pairCount, -1);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(LhsCountIsZero);
        Assert.Throws<ArgumentOutOfRangeException>(LhsCountIsNegative);
        Assert.Throws<ArgumentOutOfRangeException>(RhsCountIsZero);
        Assert.Throws<ArgumentOutOfRangeException>(RhsCountIsNegative);
        Assert.Throws<ArgumentOutOfRangeException>(PairCountIsZero);
        Assert.Throws<ArgumentOutOfRangeException>(PairCountIsNegative);
        Assert.Throws<ArgumentOutOfRangeException>(TransactionCountIsZero);
        Assert.Throws<ArgumentOutOfRangeException>(TransactionCountIsNegative);
    }

    [Fact]
    public void Ctor_FrequenciesRelationsIsInvalid_ThrowsArgumentOutOfRangeException()
    {
        // Act
        void LhsCountIsGreaterThanTransactionCount() =>
            new AssociationRule(_lhsItem, _rhsItem, _transactionCount + 1, _rhsCount, _pairCount, _transactionCount);

        void RhsCountIsGreaterThanTransactionCount() =>
            new AssociationRule(_lhsItem, _rhsItem, _lhsCount, _transactionCount + 1, _pairCount, _transactionCount);

        void PairCountIsGreaterThanLhsCount() =>
            new AssociationRule(_lhsItem, _rhsItem, _pairCount - 1, _rhsCount, _pairCount, _transactionCount);

        void PairCountIsGreaterThanRhsCount() =>
            new AssociationRule(_lhsItem, _rhsItem, _lhsCount, _pairCount - 1, _pairCount, _transactionCount);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(LhsCountIsGreaterThanTransactionCount);
        Assert.Throws<ArgumentOutOfRangeException>(RhsCountIsGreaterThanTransactionCount);
        Assert.Throws<ArgumentOutOfRangeException>(PairCountIsGreaterThanLhsCount);
        Assert.Throws<ArgumentOutOfRangeException>(PairCountIsGreaterThanRhsCount);
    }

    [Fact]
    public void Ctor_HandSidesAreSame_ThrowsArgumentException()
    {
        // Act
        void Act() =>
            new AssociationRule(_lhsItem, _lhsItem, _lhsCount, _rhsCount, _pairCount, _transactionCount);

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Ctor_ValidArguments_CreatesAssociationRule()
    {
        // Act
        var rule = new AssociationRule(_lhsItem, _rhsItem, _lhsCount, _rhsCount, _pairCount, _transactionCount);

        // Assert
        Assert.Equal(_lhsItem, rule.LeftHandSide.Item);
        Assert.Equal(_rhsItem, rule.RightHandSide.Item);
        Assert.Equal(_lhsCount, rule.LeftHandSide.Count);
        Assert.Equal(_rhsCount, rule.RightHandSide.Count);
        Assert.Equal(_pairCount, rule.PairCount);
        Assert.Equal(_support, rule.Support, _precision);
        Assert.Equal(_confidence, rule.Confidence, _precision);
        Assert.Equal(_lift, rule.Lift, _precision);
        Assert.Equal(_conviction, rule.Conviction, _precision);
        Assert.Equal(_yuleQCoefficient, rule.YuleQCoefficient, _precision);
        Assert.Equal(_phiCorrelationCoefficient, rule.PhiCorrelationCoefficient, _precision);
        Assert.Equal(_chiSquaredTestStatistic, rule.ChiSquaredTestStatistic, _precision);
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        // Arrange
        var rule = new AssociationRule(_lhsItem, _rhsItem, _lhsCount, _rhsCount, _pairCount, _transactionCount);

        // Act
        var result = rule.Equals(null);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_SameRule_ReturnsTrue()
    {
        // Arrange
        var rule = new AssociationRule(_lhsItem, _rhsItem, _lhsCount, _rhsCount, _pairCount, _transactionCount);

        // Act
        // ReSharper disable once EqualExpressionComparison
        var result = rule.Equals(rule);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_RulesWithSameHandSides_ReturnsTrue()
    {
        // Arrange
        var rule1 = new AssociationRule(_lhsItem, _rhsItem, _lhsCount, _rhsCount, _pairCount, _transactionCount);
        var rule2 = new AssociationRule(_lhsItem, _rhsItem, _lhsCount + 1, _rhsCount + 1, _pairCount + 1, _transactionCount + 1);

        // Act
        var result = rule1.Equals(rule2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_RulesWithOppositeDirections_ReturnsFalse()
    {
        // Arrange
        var rule1 = new AssociationRule(_lhsItem, _rhsItem, _lhsCount, _rhsCount, _pairCount, _transactionCount);
        var rule2 = new AssociationRule(_rhsItem, _lhsItem, _rhsCount, _lhsCount, _pairCount, _transactionCount);

        // Act
        var result = rule1.Equals(rule2);

        // Assert
        Assert.False(result);
    }
}