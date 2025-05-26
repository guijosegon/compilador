using System.Text.RegularExpressions;

namespace CompiladorParaConsole
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
            List<string> linhasProcessadas = new List<string>();
            string[] linhasOriginais = codigo.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            foreach (string linhaOriginal in linhasOriginais)
            {
                // Remove comments starting with //
                linhasProcessadas.Add(Regex.Replace(linhaOriginal, @"//.*", ""));
            }
            codigo = string.Join("\n", linhasProcessadas);


            // Regra léxica para literal -> Deve estar entre aspas duplas
            // Updated Regex to better handle simple strings, but still basic.
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
                // Split considering operators as delimiters and keeping them
                string[] palavras = Regex.Split(linhas[i], @"(\s+|>=|<=|:=|<>|[();:.,+\-*/=<>{}])");

                foreach (string palavra in palavras)
                {
                    string lexema = palavra.Trim();
                    if (string.IsNullOrEmpty(lexema)) continue;

                    if (tabela.ContainsKey(lexema))
                        resultado.Add(new TokenInfo(tabela[lexema], lexema, i + 1));
                    else if (regexString.IsMatch(lexema))
                        resultado.Add(new TokenInfo(13, lexema, i + 1));
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
            if (Regex.IsMatch(lexema, @"^\d+[a-zA-Z_].*$"))
                return $"Número malformado: 	'{lexema}' (número seguido por caracteres inválidos)";

            if (lexema.StartsWith("\"") && (!lexema.EndsWith("\"") || lexema.Length == 1))
                return $"String não finalizada: 	'{lexema}' (aspas de fechamento ausente ou string vazia inválida)";

            if (Regex.IsMatch(lexema, @"\d+\.\.\d+") || Regex.IsMatch(lexema, @"^\.\d+") || Regex.IsMatch(lexema, @"\d+\.$$"))
                return $"Número real inválido: 	'{lexema}' (formato incorreto, ex: '..', '.5', '5.')";

            if (Regex.IsMatch(lexema, @"^[0-9]"))
            {
                if (!Regex.IsMatch(lexema, @"^\d+$") && !Regex.IsMatch(lexema, @"^\d+\.\d+$"))
                    return $"Número malformado: 	'{lexema}'";
            }

            if (!Regex.IsMatch(lexema, @"^[a-zA-Z0-9_+*/=<>():;.,{}""\s.-]+$"))
                return $"Caractere inválido: 	'{lexema}' (contém símbolos não permitidos pela linguagem)";

            if (lexema.Contains("@"))
                return $"Identificador inválido: 	'{lexema}' (caractere '@' não permitido)";

            if (lexema == ":=:")
                return $"Operador malformado: 	'{lexema}'";

            return $"Lexema inválido: 	'{lexema}'";
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