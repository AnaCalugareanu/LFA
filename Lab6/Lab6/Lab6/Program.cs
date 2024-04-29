using System.Text;
using System.Text.RegularExpressions;

internal enum TokenType
{
    OPEN_PARENTHESIS,
    CLOSE_PARENTHESIS,
    MATH_OPERATION,
    NUMBERS,
    START
}

internal class Lexer
{
    private string equation;

    public Lexer(string equation)
    {
        this.equation = equation.Replace(" ", "");
    }

    public (List<TokenType>, List<string>) Tokenize()
    {
        Dictionary<TokenType, List<string>> data = new Dictionary<TokenType, List<string>>
        {
            { TokenType.OPEN_PARENTHESIS, new List<string> { "\\(", "\\[" } },
            { TokenType.CLOSE_PARENTHESIS, new List<string> { "\\)", "\\]" } },
            { TokenType.MATH_OPERATION, new List<string> { "[+\\-*/%^]" } },
            { TokenType.NUMBERS, new List<string> { "\\d+" } }
        };

        Dictionary<TokenType, List<TokenType>> transitions = new Dictionary<TokenType, List<TokenType>>
        {
            { TokenType.OPEN_PARENTHESIS, new List<TokenType> { TokenType.NUMBERS, TokenType.OPEN_PARENTHESIS } },
            { TokenType.MATH_OPERATION, new List<TokenType> { TokenType.NUMBERS, TokenType.OPEN_PARENTHESIS } },
            { TokenType.CLOSE_PARENTHESIS, new List<TokenType> { TokenType.MATH_OPERATION, TokenType.CLOSE_PARENTHESIS } },
            { TokenType.NUMBERS, new List<TokenType> { TokenType.NUMBERS, TokenType.CLOSE_PARENTHESIS, TokenType.MATH_OPERATION } },
            { TokenType.START, new List<TokenType> { TokenType.OPEN_PARENTHESIS, TokenType.NUMBERS } }
        };

        Stack<string> seqParenthesis = new Stack<string>();
        List<TokenType> categoryMapping = new List<TokenType> { TokenType.START };
        List<string> validTokens = new List<string>();
        StringBuilder failedOn = new StringBuilder();

        foreach (char symbol in this.equation)
        {
            string symStr = symbol.ToString();
            bool foundCategory = false;
            TokenType? currentCategory = null;

            foreach (var category in data.Keys)
            {
                foreach (var pattern in data[category])
                {
                    if (Regex.IsMatch(symStr, pattern))
                    {
                        currentCategory = category;
                        foundCategory = true;
                        break;
                    }
                }
                if (foundCategory)
                    break;
            }

            if (!foundCategory)
            {
                Console.WriteLine($"ERROR: Symbol '{symStr}' does not belong to any known category.");
                Console.WriteLine($"Failed on symbol {failedOn}");
                return (null, null);
            }

            if (!transitions[categoryMapping.Last()].Contains(currentCategory.Value))
            {
                Console.WriteLine($"ERROR: Transition not allowed from '{categoryMapping.Last()}' to '{currentCategory}'.");
                Console.WriteLine($"Failed on symbol {failedOn}");
                return (null, null);
            }

            categoryMapping.Add(currentCategory.Value);
            validTokens.Add(symStr);
            failedOn.Append(symStr);

            if (currentCategory == TokenType.OPEN_PARENTHESIS)
                seqParenthesis.Push(symStr);
            else if (currentCategory == TokenType.CLOSE_PARENTHESIS)
            {
                if (seqParenthesis.Count == 0)
                {
                    Console.WriteLine("ERROR: Extra closing parenthesis found.");
                    Console.WriteLine($"Failed on symbol {failedOn}");
                    return (null, null);
                }

                string lastOpen = seqParenthesis.Pop();

                if ((symStr == ")" && lastOpen != "(") || (symStr == "]" && lastOpen != "["))
                {
                    Console.WriteLine("ERROR: Mismatched closing parenthesis found.");
                    Console.WriteLine($"Failed on symbol {failedOn}");
                    return (null, null);
                }
            }
        }

        if (seqParenthesis.Count > 0)
        {
            Console.WriteLine("ERROR: Not all parentheses were closed.");
            Console.WriteLine($"Failed on symbol {failedOn}");
            return (null, null);
        }

        return (categoryMapping, validTokens);
    }
}

internal class Parser
{
    private List<TokenType> categoryMapping;
    private List<string> validTokens;

    public Parser(List<TokenType> categoryMapping, List<string> validTokens)
    {
        this.categoryMapping = categoryMapping;
        this.validTokens = validTokens;
    }

    public void Parse()
    {
        var tree = new List<(string, string)>();

        var rootNode = categoryMapping[0].ToString();
        var parentNodes = new Stack<string>();
        parentNodes.Push(rootNode);

        for (int i = 0; i < validTokens.Count; i++)
        {
            string token = validTokens[i];
            string category = categoryMapping[i + 1].ToString();
            var newNode = $"{token} ({category})";

            tree.Add((newNode, parentNodes.Peek()));
            parentNodes.Push(newNode);
        }

        foreach (var (node, parent) in tree)
        {
            var indent = new string(' ', parent.Length * 2);
            Console.WriteLine($"{indent}{node}");
        }
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        string testEquation = "5+(7-4)/5";

        Lexer lexer = new Lexer(testEquation);
        var (categoryMapping, validTokens) = lexer.Tokenize();

        if (categoryMapping != null && validTokens != null)
        {
            Console.WriteLine("Category Mapping: " + string.Join(", ", categoryMapping));
            Console.WriteLine("Valid Tokens: " + string.Join(", ", validTokens));

            Parser parser = new Parser(categoryMapping, validTokens);
            parser.Parse();
        }
    }
}