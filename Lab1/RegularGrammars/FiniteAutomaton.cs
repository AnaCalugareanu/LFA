public class FiniteAutomaton
{
    public HashSet<string> Q;
    public HashSet<char> Sigma;
    public Dictionary<(string, char), HashSet<string>> Delta;
    public HashSet<string> q0;
    public HashSet<string> F;

    public FiniteAutomaton(HashSet<string> q, HashSet<char> sigma, Dictionary<(string, char), HashSet<string>> delta, HashSet<string> initial, HashSet<string> f)
    {
        Q = q;
        Sigma = sigma;
        Delta = delta;
        q0 = initial;
        F = f;
    }

    public bool StringBelongsToLanguage(string w)
    {
        var currentStates = new HashSet<string>(q0);
        foreach (var letter in w)
        {
            var nextStates = new HashSet<string>();
            foreach (var state in currentStates)
            {
                if (Delta.TryGetValue((state, letter), out var states))
                {
                    nextStates.UnionWith(states);
                }
            }
            currentStates = nextStates;
        }
        return currentStates.Any(state => F.Contains(state));
    }
}