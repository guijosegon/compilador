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
                {"while", 1}, {"var", 2}, {"to", 3}, {"then", 4},{"string", 5}, {"real", 6}, {"read", 7}, {"program", 8},
                {"procedure", 9}, {"print", 10}, {"nreal", 11}, {"nint", 12},{"literal", 13}, {"integer", 14}, {"if", 15}, 
                {"ident", 16},{"for", 17}, {"end", 18}, {"else", 19}, {"do", 20}, {"const", 21}, {"begin", 22}, {"vstring", 23}, 
                {">=", 24}, {">", 25}, {"=", 26}, {"<>", 27}, {"<=", 28}, {"<", 29}, {"+", 30}, {";", 31}, {":=", 32}, {":", 33},
                { "/", 34}, {".", 35}, {",", 36}, {"*", 37}, {")", 38}, {"(", 39}, {"{", 40}, {"}", 41}, {"-", 42}, {"$", 43}, {"î", 44}
            };

            int indexComentarioAberto = codigo.IndexOf("/*");
            while (indexComentarioAberto != -1)
            {
                int indexComentarioFechado = codigo.IndexOf("*/", indexComentarioAberto + 2);
                if (indexComentarioFechado == -1)
                {
                    int linhaComentario = codigo.Substring(0, indexComentarioAberto).Split('\n').Length;
                    Erros.Add($"[ERRO] Comentário multilinha não fechado na linha {linhaComentario}");
                    codigo = codigo.Substring(0, indexComentarioAberto);
                    break;
                }
                else
                {
                    codigo = codigo.Remove(indexComentarioAberto, indexComentarioFechado - indexComentarioAberto + 2);
                    indexComentarioAberto = codigo.IndexOf("/*");
                }
            }

            List<string> linhasProcessadas = new List<string>();
            string[] linhasOriginais = codigo.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            foreach (string linhaOriginal in linhasOriginais)
            {
                linhasProcessadas.Add(Regex.Replace(linhaOriginal, @"//.*", ""));
            }
            codigo = string.Join("\n", linhasProcessadas);

            string[] linhas = codigo.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

            for (int i = 0; i < linhas.Length; i++)
            {
                string linha = linhas[i];
                int coluna = 0;

                while (coluna < linha.Length)
                {
                    char atual = linha[coluna];

                    if (char.IsWhiteSpace(atual))
                    {
                        coluna++;
                        continue;
                    }

                    // Número real com validação de casos mal formados
                    var matchFloat = Regex.Match(linha.Substring(coluna), @"^\d+\.\d+");
                    if (matchFloat.Success)
                    {
                        string lexema = matchFloat.Value;
                        resultado.Add(new TokenInfo(11, lexema, i + 1));
                        coluna += lexema.Length;
                        continue;
                    }

                    // Número real mal formado tipo 123. (ponto sem decimais)
                    var matchFloatMalFormadoFim = Regex.Match(linha.Substring(coluna), @"^\d+\.");
                    if (matchFloatMalFormadoFim.Success)
                    {
                        string lexema = matchFloatMalFormadoFim.Value;
                        Erros.Add($"[ERRO] Número real mal formado (falta parte decimal): '{lexema}' na linha {i + 1}");
                        coluna += lexema.Length;
                        continue;
                    }

                    // Número real mal formado tipo .123 (ponto sem parte inteira)
                    var matchFloatMalFormadoInicio = Regex.Match(linha.Substring(coluna), @"^\.\d+");
                    if (matchFloatMalFormadoInicio.Success)
                    {
                        string lexema = matchFloatMalFormadoInicio.Value;
                        Erros.Add($"[ERRO] Número real mal formado (falta parte inteira): '{lexema}' na linha {i + 1}");
                        coluna += lexema.Length;
                        continue;
                    }

                    // Número inteiro
                    var matchInt = Regex.Match(linha.Substring(coluna), @"^\d+");
                    if (matchInt.Success)
                    {
                        string lexema = matchInt.Value;
                        // Verificar se número seguido por letra (ex: 123a)
                        if (coluna + lexema.Length < linha.Length && char.IsLetter(linha[coluna + lexema.Length]))
                        {
                            Erros.Add($"[ERRO] Número seguido de caractere inválido: '{linha.Substring(coluna, lexema.Length + 1)}' na linha {i + 1}");
                            coluna += lexema.Length + 1;
                            continue;
                        }

                        resultado.Add(new TokenInfo(12, lexema, i + 1));
                        coluna += lexema.Length;
                        continue;
                    }

                    // String com aspas duplas
                    if (linha[coluna] == '"')
                    {
                        int fimString = coluna + 1;
                        bool escapando = false;
                        bool erroString = false;
                        while (fimString < linha.Length)
                        {
                            char c = linha[fimString];

                            if (escapando)
                            {
                                // Aceitar escapes comuns, rejeitar escapes inválidos
                                if ("\"\\nrt".IndexOf(c) == -1)
                                {
                                    Erros.Add($"[ERRO] Sequência de escape inválida: '\\{c}' na linha {i + 1}");
                                    erroString = true;
                                }
                                escapando = false;
                            }
                            else if (c == '\\')
                            {
                                escapando = true;
                            }
                            else if (c == '"')
                            {
                                break; // fim da string
                            }
                            fimString++;
                        }

                        if (fimString >= linha.Length || linha[fimString] != '"')
                        {
                            Erros.Add($"[ERRO] String sem aspas finais na linha {i + 1}");
                            coluna = linha.Length; // Pular o resto da linha
                            continue;
                        }

                        if (!erroString)
                        {
                            string lexema = linha.Substring(coluna, fimString - coluna + 1);
                            resultado.Add(new TokenInfo(13, lexema, i + 1));
                        }

                        coluna = fimString + 1;
                        continue;
                    }

                    // Operadores compostos
                    string[] compostos = { ">=", "<=", ":=", "<>" };
                    bool encontrouComposto = false;
                    foreach (var op in compostos)
                    {
                        if (linha.Substring(coluna).StartsWith(op))
                        {
                            resultado.Add(new TokenInfo(tabela[op], op, i + 1));
                            coluna += op.Length;
                            encontrouComposto = true;
                            break;
                        }
                    }
                    if (encontrouComposto) continue;

                    // Símbolos simples
                    string simbolo = linha[coluna].ToString();
                    if (tabela.ContainsKey(simbolo))
                    {
                        resultado.Add(new TokenInfo(tabela[simbolo], simbolo, i + 1));
                        coluna++;
                        continue;
                    }

                    // Identificadores (letras ou _ no começo)
                    var matchIdent = Regex.Match(linha.Substring(coluna), @"^[a-zA-Z_][a-zA-Z0-9_]*");
                    if (matchIdent.Success)
                    {
                        string lexema = matchIdent.Value;
                        if (lexema.Length > 30)
                        {
                            Erros.Add($"[ERRO] Identificador muito longo (>30 caracteres): '{lexema}' na linha {i + 1}");
                        }
                        int cod = tabela.ContainsKey(lexema) ? tabela[lexema] : 16;
                        resultado.Add(new TokenInfo(cod, lexema, i + 1));
                        coluna += lexema.Length;
                        continue;
                    }

                    // Se chegou aqui, é lexema inválido
                    string resto = linha.Substring(coluna).Split(' ').FirstOrDefault() ?? linha[coluna].ToString();
                    Erros.Add($"[ERRO] Lexema inválido: '{resto}' na linha {i + 1}");
                    coluna += resto.Length;
                }
            }

            return resultado;
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