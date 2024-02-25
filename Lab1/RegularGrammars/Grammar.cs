public class Grammar
{
    public HashSet<char> VN;
    public HashSet<char> VT;
    public Dictionary<char, List<string>> P;
    public char S;

    private Random rnd = new Random();

    public Grammar(HashSet<char> vn, HashSet<char> vt, Dictionary<char, List<string>> p, char s)
    {
        VN = vn;
        VT = vt;
        P = p;
        S = s;
    }

    public List<string> GenerateValidStrings(int count)
    {
        List<string> validStrings = new List<string>();

        string GenerateFromSymbol(char symbol)
        {
            if (VT.Contains(symbol))
            {
                return symbol.ToString();
            }
            else
            {
                var production = P[symbol][rnd.Next(P[symbol].Count)];
                return string.Concat(production.Select(GenerateFromSymbol));
            }
        }

        for (int i = 0; i < count; i++)
        {
            validStrings.Add(GenerateFromSymbol(S));
        }

        return validStrings;
    }

    public List<string> GenerateRandomStrings(int count, int maxLength)
    {
        List<string> strings = new List<string>();

        string GenerateFromSymbol(char symbol, int currentLength)
        {
            if (currentLength > 0 && rnd.Next(0, maxLength) < currentLength)
                return VN.Contains(symbol) ? "" : symbol.ToString();

            if (currentLength >= maxLength || (!VN.Contains(symbol) && !VT.Contains(symbol)))
                return symbol.ToString();

            if (rnd.Next(2) == 0 && P.ContainsKey(symbol))
            {
                var production = P[symbol][rnd.Next(P[symbol].Count)];
                return string.Concat(production.Select(s => GenerateFromSymbol(s, currentLength + 1)));
            }
            else
            {
                var allSymbols = VN.Union(VT).ToList();
                var randomSymbol = allSymbols[rnd.Next(allSymbols.Count)];
                return GenerateFromSymbol(randomSymbol, currentLength + 1) + (VN.Contains(symbol) ? "" : symbol.ToString());
            }
        }

        for (int i = 0; i < count; i++)
        {
            strings.Add(GenerateFromSymbol(S, 0));
        }

        return strings;
    }

    public FiniteAutomaton ToFiniteAutomaton()
    {
        HashSet<string> Q = new HashSet<string>(VN.Select(v => v.ToString())).Union(new[] { "X" }).ToHashSet();
        var Sigma = VT;
        var Delta = new Dictionary<(string, char), HashSet<string>>();
        var q0 = new HashSet<string> { S.ToString() };
        var F = new HashSet<string> { "X" };

        foreach (var state in Q)
        {
            foreach (var symbol in Sigma)
            {
                Delta[(state, symbol)] = new HashSet<string>();
            }
        }

        foreach (var entry in P)
        {
            foreach (var production in entry.Value)
            {
                if (production.Length == 1 && VT.Contains(production[0]))
                {
                    Delta[(entry.Key.ToString(), production[0])].Add("X");
                }
                else if (production.Length > 0 && VT.Contains(production[0]))
                {
                    var nextState = production.Length > 1 ? production[1].ToString() : "X";
                    Delta[(entry.Key.ToString(), production[0])].Add(nextState);
                }
            }
        }

        return new FiniteAutomaton(Q, Sigma, Delta, q0, F);
    }
}