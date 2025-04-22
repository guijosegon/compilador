using System.Text.RegularExpressions;

namespace CompiladorComInterface
{
    public static class AnalisadorLexico
    {
        public static List<string> Erros { get; private set; } = new List<string>();

        public static List<TokenInfo> Analisar(string codigo)
        {
            Erros.Clear();
            List<TokenInfo> resultado = new List<TokenInfo>();

            Dictionary<string, int> tabela = new Dictionary<string, int>
            {
                {"while", 1},
                {"var", 2},
                {"to", 3},
                {"then", 4},
                {"string", 5},
                {"real", 6},
                {"read", 7},
                {"program", 8},
                {"procedure", 9},
                {"print", 10},
                {"nreal", 11},
                {"nint", 12},
                {"literal", 13},
                {"integer", 14},
                {"if", 15},
                {"ident", 16},
                {"for", 17},
                {"end", 18},
                {"else", 19},
                {"do", 20},
                {"const", 21},
                {"begin", 22},
                {"vstring", 23},
                {">=", 24},
                {">", 25},
                {"=", 26},
                {"<>", 27},
                {"<=", 28},
                {"<", 29},
                {"+", 30},
                {";", 31},
                {":=", 32},
                {":", 33},
                {"/", 34},
                {".", 35},
                {",", 36},
                {"*", 37},
                {")", 38},
                {"(", 39},
                {"{", 40},
                {"}", 41},
                {"-", 42},
                {"$", 43},
                {"î", 44}
            };

            // Regra léxica para comentário -> Deve começar com // e sua linha removida.
            codigo = Regex.Replace(codigo, @"//.*", "");

            // Regra léxica para literal -> Deve estar entre aspas duplas e podem conter letras, números, espaços e símbolos (menos aspas não escapadas)
            Regex regexString = new Regex("^\".*\"$");

            // Regra léxica para nreal -> Devem conter parte inteira, ponto e parte decimal.
            Regex regexFloat = new Regex(@"^\d+\.\d+$");

            // Regra léxica para nint -> Devem conter apenas dígitos.
            Regex regexInt = new Regex(@"^\d+$");

            // Regra léxica para ident -> Devem começar com letra (a-z ou A-Z) ou underscore _, podem conter numeros.
            Regex regexIdent = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");

            string[] linhas = codigo.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            for (int i = 0; i < linhas.Length; i++)
            {
                string linhaTexto = Regex.Replace(linhas[i], @"//.*", "");
                string[] palavras = Regex.Split(linhaTexto, @"(\s+|>=|<=|:=|<>|[();:.,+\-*/=<>{}])");

                foreach (string palavra in palavras)
                {
                    string lexema = palavra.Trim();
                    if (string.IsNullOrEmpty(lexema)) continue;

                    if (regexString.IsMatch(lexema))
                        resultado.Add(new TokenInfo(13, lexema, i + 1));
                    else if (tabela.ContainsKey(lexema))
                        resultado.Add(new TokenInfo(tabela[lexema], lexema, i + 1));
                    else if (regexFloat.IsMatch(lexema))
                        resultado.Add(new TokenInfo(11, lexema, i + 1));
                    else if (regexInt.IsMatch(lexema))
                        resultado.Add(new TokenInfo(12, lexema, i + 1));
                    else if (regexIdent.IsMatch(lexema))
                        resultado.Add(new TokenInfo(16, lexema, i + 1));
                    else
                        Erros.Add($"[ERRO] {DetectarErroLexico(lexema)} na linha {i + 1}");
                }
            }

            return resultado;
        }

        private static string DetectarErroLexico(string lexema)
        {
            if (Regex.IsMatch(lexema, @"^\d+[a-zA-Z_]+"))
                return $"Número malformado: '{lexema}' (ex: número seguido por letras como '123abc')";

            if (lexema.StartsWith("\"") && !lexema.EndsWith("\""))
                return $"String não finalizada: '{lexema}' (aspas de fechamento ausente)";

            if (lexema.Contains(".."))
                return $"Número real inválido: '{lexema}' (dois pontos consecutivos)";

            if (lexema.Contains("@"))
                return $"Identificador inválido: '{lexema}' (caractere '@' não permitido)";

            if (Regex.IsMatch(lexema, @":=.+"))
                return $"Operador malformado: '{lexema}' (ex: ':=:' inválido)";

            return $"Lexema inválido: '{lexema}'";
        }
    }

    public class TokenInfo
    {
        public int Codigo { get; set; }
        public string Lexema { get; set; }
        public int Linha { get; set; }

        public TokenInfo(int codigo, string lexema, int linha)
        {
            Codigo = codigo;
            Lexema = lexema;
            Linha = linha;
        }
    }
}
