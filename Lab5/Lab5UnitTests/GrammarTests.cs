using Xunit;

public class GrammarTests
{
    private readonly Grammar grammar;

    public GrammarTests()
    {
        grammar = new Grammar();
    }

    [Fact]
    public void TestRemoveEpsilon()
    {
        var result = grammar.RemoveEpsilon();
        var expected = new Dictionary<char, List<string>>
        {
            {'S', new List<string> {"aB", "bA", "A", "a"}},
            {'A', new List<string> {"b", "B", "AS", "bBAB", "bA"}},
            {'B', new List<string> {"b", "bS", "aD"}},
            {'D', new List<string> {"AA"}},
            {'C', new List<string> {"Ba", "a"}}
        };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestEliminateUnitProd()
    {
        grammar.RemoveEpsilon();
        var result = grammar.EliminateUnitProd();
        var expected = new Dictionary<char, List<string>>
        {
            {'S', new List<string> {"aB", "bA", "a", "b", "B"}},
            {'A', new List<string> {"b", "AS", "bBAB", "bA", "b",}},
            {'B', new List<string> {"b", "bS", "aD"}},
            {'D', new List<string> {"AA"}},
            {'C', new List<string> {"Ba", "a"}}
        };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestEliminateInaccessible()
    {
        grammar.RemoveEpsilon();
        grammar.EliminateUnitProd();
        var result = grammar.EliminateInaccessible();
        var expected = new Dictionary<char, List<string>>
        {
            {'S', new List<string> {"aB", "bA", "b", "B", "AS", "bBAB"}},
            {'A', new List<string> {"b", "bS", "aD"}},
            {'B', new List<string> {"b", "bS", "aD"}},
            {'D', new List<string> {"b", "B", "AS", "bBAB"}}
        };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestRemoveUnprod()
    {
        grammar.RemoveEpsilon();
        grammar.EliminateUnitProd();
        grammar.EliminateInaccessible();
        var result = grammar.RemoveUnprod();
        var expected = new Dictionary<char, List<string>>
        {
            {'S', new List<string> {"aB", "bA", "b", "B", "AS", "bBAB"}},
            {'A', new List<string> {"b", "bS", "aD"}},
            {'B', new List<string> {"b", "bS", "aD"}},
            {'D', new List<string> {"b", "B", "AS", "bBAB"}}
        };

        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestTransformToCNF()
    {
        grammar.RemoveEpsilon();
        grammar.EliminateUnitProd();
        grammar.EliminateInaccessible();
        grammar.RemoveUnprod();
        var result = grammar.TransformToCNF();
        var expected = new Dictionary<char, List<string>>
        {
            {'S', new List<string> {"AB", "BA", "b", "B1A2", "bB1A3B4"}},
            // Continue with all transformed productions
        };

        Assert.Equal(expected, result);
    }
}