using CompiladorParaConsole;

class Program
{
    static void Main()
    {
        string caminhoArquivo = "";

        Console.WriteLine("Escolha o arquivo de exemplo para analisar:");
        Console.WriteLine("1: exemplo1.txt (Declarações, I/O, Comentários)");
        Console.WriteLine("2: exemplo2.txt (Procedures, If/Else, While)");
        Console.WriteLine("3: exemplo3.txt (For, Expressões)");
        Console.Write("Digite o número da opção: ");

        string escolha = Console.ReadLine();

        switch (escolha)
        {
            case "1":
                caminhoArquivo = "exemplo1.txt";
                break;
            case "2":
                caminhoArquivo = "exemplo2.txt";
                break;
            case "3":
                caminhoArquivo = "exemplo3.txt";
                break;
            default:
                Console.WriteLine("Opção inválida. Usando 'exemplo1.txt' como padrão.");
                caminhoArquivo = "exemplo1.txt";
                break;
        }

        if (!File.Exists(caminhoArquivo))
        {
            Console.WriteLine($"\n[ERRO] Arquivo não encontrado: {caminhoArquivo}");
            return;
        }

        Console.WriteLine($"\nAnalisando arquivo: {caminhoArquivo}...");
        string codigo = File.ReadAllText(caminhoArquivo);

        // --- Análise Léxica ---
        Console.WriteLine("\n--- Análise Léxica ---");
        var tokens = AnalisadorLexico.Analisar(codigo);

        Console.WriteLine("\n--- Tokens Identificados ---");
        if (tokens.Any())
        {
            foreach (var token in tokens)
                Console.WriteLine($"Token: {token.Codigo,-3} | Lexema: 	'{token.Lexema}'	 | Linha: {token.Linha}");
        }
        else
        {
            Console.WriteLine("Nenhum token identificado.");
        }

        if (AnalisadorLexico.Erros.Count > 0)
        {
            Console.WriteLine("\n--- Erros Léxicos Detectados ---");
            AnalisadorLexico.Erros.ForEach(e => Console.WriteLine(e));
            Console.WriteLine("\nAnálise sintática não pode prosseguir devido a erros léxicos.");
        }
        else
        {
            Console.WriteLine("\nNenhum erro léxico encontrado.");

            // --- Análise Sintática ---
            AnalisadorSintatico analisadorSintatico = new AnalisadorSintatico();
            bool sucessoSintatico = analisadorSintatico.Analisar(tokens);

            if (!sucessoSintatico)
            {
                // Erros já são mostrados dentro do método Analisar/MostrarErros
                // Console.WriteLine("\nAnálise sintática falhou.");
            }
        }

        Console.WriteLine("\nAnálise concluída. Pressione Enter para sair.");
        Console.ReadLine();
    }
}
