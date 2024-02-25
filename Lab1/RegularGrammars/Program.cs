namespace RegularGrammars
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var VN = new HashSet<char> { 'S', 'L', 'D' };
            var VT = new HashSet<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'j' };
            var P = new Dictionary<char, List<string>>
        {
            { 'S', new List<string> { "aS", "bS", "cD", "dL", "e" } },
            { 'L', new List<string> { "eL", "fL", "jD", "e" } },
            { 'D', new List<string> { "eD", "d" } }
        };
            char S = 'S';

            var grammar = new Grammar(VN, VT, P, S);
            var validStrings = grammar.GenerateValidStrings(5);

            Console.WriteLine("Generated valid strings from the Grammar rules:");
            foreach (var str in validStrings)
            {
                Console.WriteLine(str);
            }

            for (int i = 0; i < 100; i++)
                Console.Write('.');
            Console.WriteLine();

            var randomStrings = grammar.GenerateRandomStrings(20, 5);

            var finiteAutomaton = grammar.ToFiniteAutomaton();

            Console.WriteLine("Generated random strings verified by FA:\n");
            foreach (var word in randomStrings)
            {
                Console.WriteLine($"{word} can be obtained via the state transition: {finiteAutomaton.StringBelongsToLanguage(word)}");
            }
        }
    }
}