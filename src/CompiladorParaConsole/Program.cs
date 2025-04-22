using CompiladorComInterface;

class Program
{
    static void Main()
    {
        string caminhoArquivo = "codigo.txt";

        if (!File.Exists(caminhoArquivo))
        {
            Console.WriteLine($"Arquivo não encontrado: {caminhoArquivo}");
            return;
        }

        string codigo = File.ReadAllText(caminhoArquivo);
        var tokens = CompiladorComInterface.AnalisadorLexico.Analisar(codigo);

        foreach (var token in tokens)
            Console.WriteLine($"Token: {token.Codigo} - Lexema: {token.Lexema} - Linha: {token.Linha}");

        if (CompiladorComInterface.AnalisadorLexico.Erros.Count > 0)
        {
            Console.WriteLine("\nErros léxicos:");
            CompiladorComInterface.AnalisadorLexico.Erros.ForEach(e => Console.WriteLine(e));
        }
    }
}