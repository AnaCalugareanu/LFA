public class Grammar
{
    private Dictionary<char, List<string>> P;
    private List<char> V_N;
    private List<char> V_T;

    public Grammar()
    {
        P = new Dictionary<char, List<string>>
        {
            {'S', new List<string> {"aB", "bA", "A"}},
            {'A', new List<string> {"b", "B", "AS", "bBAB"}},
            {'B', new List<string> {"b", "bS", "aD", "eps"}},
            {'D', new List<string> {"AA"}},
            {'C', new List<string> {"Ba"}}
        };
        V_N = new List<char> { 'S', 'A', 'B', 'D', 'C' };
        V_T = new List<char> { 'a', 'b' };
    }

    public Dictionary<char, List<string>> RemoveEpsilon()
    {
        var ntEpsilon = new List<char>();
        foreach (var entry in P)
        {
            if (entry.Value.Contains("eps"))
            {
                ntEpsilon.Add(entry.Key);
            }
        }

        foreach (var ep in ntEpsilon)
        {
            foreach (var entry in P.ToList())
            {
                for (int i = 0; i < entry.Value.Count; i++)
                {
                    var prod = entry.Value[i];
                    if (prod.Contains(ep))
                    {
                        string newProd = prod.Replace(ep.ToString(), "");
                        if (!string.IsNullOrEmpty(newProd))
                        {
                            P[entry.Key].Add(newProd);
                        }
                    }
                }
            }
            P[ep].Remove("eps");
        }

        Console.WriteLine("1. After removing epsilon productions:");
        PrintProductions();
        return new Dictionary<char, List<string>>(P);
    }

    public Dictionary<char, List<string>> EliminateUnitProd()
    {
        var newP = new Dictionary<char, List<string>>(P);
        foreach (var entry in P)
        {
            foreach (var prod in entry.Value.ToList())
            {
                if (prod.Length == 1 && V_N.Contains(prod[0]))
                {
                    newP[entry.Key].Remove(prod);
                    newP[entry.Key].AddRange(P[prod[0]]);
                }
            }
        }

        P = newP;
        Console.WriteLine("2. After removing unit productions:");
        PrintProductions();
        return new Dictionary<char, List<string>>(P);
    }

    public Dictionary<char, List<string>> EliminateInaccessible()
    {
        var accessible = new HashSet<char> { 'S' }; // Start symbol is always accessible
        bool added;
        do
        {
            added = false;
            foreach (var entry in P)
            {
                if (accessible.Contains(entry.Key))
                {
                    foreach (var prod in entry.Value)
                    {
                        foreach (var symbol in prod)
                        {
                            if (char.IsUpper(symbol) && !accessible.Contains(symbol))
                            {
                                accessible.Add(symbol);
                                added = true;
                            }
                        }
                    }
                }
            }
        } while (added);

        var newP = P.Where(entry => accessible.Contains(entry.Key)).ToDictionary(entry => entry.Key, entry => entry.Value);
        P = newP;

        Console.WriteLine("3. After removing inaccessible symbols:");
        PrintProductions();
        return new Dictionary<char, List<string>>(P);
    }

    public Dictionary<char, List<string>> RemoveUnprod()
    {
        var productive = new HashSet<char>(V_T);
        bool added;
        do
        {
            added = false;
            foreach (var entry in P)
            {
                if (entry.Value.Any(prod => prod.All(c => productive.Contains(c) || !char.IsUpper(c))) && !productive.Contains(entry.Key))
                {
                    productive.Add(entry.Key);
                    added = true;
                }
            }
        } while (added);

        var newP = P.Where(entry => productive.Contains(entry.Key)).ToDictionary(entry => entry.Key, entry => entry.Value);
        P = newP;

        Console.WriteLine("4. After removing unproductive symbols:");
        PrintProductions();
        return new Dictionary<char, List<string>>(P);
    }

    public Dictionary<char, List<string>> TransformToCNF()
    {
        Dictionary<char, List<string>> cnf = new Dictionary<char, List<string>>(P);
        Dictionary<string, char> newVariables = new Dictionary<string, char>();
        char nextFreeVariable = 'Z';

        foreach (var entry in P)
        {
            var newProductions = new List<string>();
            foreach (var production in entry.Value)
            {
                if (production.Length == 1 && V_T.Contains(production[0]))
                {
                    newProductions.Add(production);
                }
                else
                {
                    var currentProduction = production;
                    while (currentProduction.Length > 2)
                    {
                        var leftPart = currentProduction.Substring(0, 2);
                        currentProduction = currentProduction.Substring(2);
                        if (!newVariables.ContainsKey(leftPart))
                        {
                            newVariables[leftPart] = nextFreeVariable--;
                            cnf[newVariables[leftPart]] = new List<string> { leftPart };
                        }
                        currentProduction = newVariables[leftPart] + currentProduction;
                    }
                    newProductions.Add(currentProduction);
                }
            }
            cnf[entry.Key] = newProductions;
        }

        P = cnf;
        Console.WriteLine("5. Final CNF:");
        PrintProductions();
        return cnf;
    }

    public void PrintProductions()
    {
        foreach (var entry in P)
        {
            Console.Write($"{entry.Key} -> ");
            Console.WriteLine(string.Join(" | ", entry.Value));
        }
    }

    public static void Main(string[] args)
    {
        Grammar g = new Grammar();
        g.RemoveEpsilon();
        g.EliminateUnitProd();
        g.EliminateInaccessible();
        g.RemoveUnprod();
        g.TransformToCNF();
    }
}