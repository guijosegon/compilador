namespace CompiladorParaConsole
{
    public class AnalisadorSemantico
    {
        public TabelaDeSimbolos Tabela { get; } = new TabelaDeSimbolos();
        public List<string> ErrosSemanticos { get; } = new List<string>();
        private Stack<string> _tiposExpressao = new Stack<string>();

        public void Acao_InserirConstante(TokenInfo identificador, TokenInfo valorToken)
        {
            int valorConstante;

            if (!int.TryParse(valorToken.Lexema, out valorConstante))
            {
                RegistrarErro($"Valor inválido para constante '{identificador.Lexema}'. Esperado um número inteiro.", identificador.Linha);
                return;
            }

            var simbolo = new Simbolo(identificador.Lexema, CategoriaSimbolo.CONSTANTE, "integer", 0, valorConstante);

            if (!Tabela.Inserir(simbolo))
                RegistrarErro($"Constante '{identificador.Lexema}' já declarada.", identificador.Linha);
        }

        public void Acao_InserirVariavel(TokenInfo identificador, string tipo)
        {
            var simbolo = new Simbolo(identificador.Lexema, CategoriaSimbolo.VARIAVEL, tipo, Tabela.NivelAtual);

            if (!Tabela.Inserir(simbolo))
                RegistrarErro($"Variável '{identificador.Lexema}' já declarada neste escopo.", identificador.Linha);
        }

        public void Acao_VerificarDeclaracao(TokenInfo identificador)
        {
            var simbolo = Tabela.Buscar(identificador.Lexema);

            if (simbolo is null)
            {
                RegistrarErro($"Identificador '{identificador.Lexema}' não foi declarado.", identificador.Linha);
                _tiposExpressao.Push("indefinido");
            }
            else
            {
                _tiposExpressao.Push(simbolo.Tipo);
            }
        }

        // Ação: Verificar tipo em uma atribuição (:=)
        public void Acao_VerificarAtribuicao(TokenInfo variavel)
        {
            var simbolo = Tabela.Buscar(variavel.Lexema);

            if (simbolo is null)
            {
                RegistrarErro($"Variável '{variavel.Lexema}' não declarada.", variavel.Linha);
                return;
            }
            if (simbolo.Categoria == CategoriaSimbolo.CONSTANTE)
            {
                RegistrarErro($"Não é possível atribuir valor a uma constante '{variavel.Lexema}'.", variavel.Linha);
            }

            string tipoExpressao = _tiposExpressao.Any() ? _tiposExpressao.Pop() : "indefinido";
            string tipoVariavel = simbolo.Tipo;

            if (tipoVariavel != tipoExpressao && !(tipoVariavel == "real" && tipoExpressao == "integer"))
                RegistrarErro($"Tipos incompatíveis. Não é possível atribuir um valor do tipo '{tipoExpressao}' a uma variável do tipo '{tipoVariavel}' ('{variavel.Lexema}').", variavel.Linha);
        }

        public void Acao_EmpilharTipo(TokenInfo token)
        {
            switch (MapearTokenParaTerminal(token))
            {
                case "nint":
                    _tiposExpressao.Push("integer");
                    break;
                case "nreal":
                    _tiposExpressao.Push("real");
                    break;
                case "literal":
                    _tiposExpressao.Push("string");
                    break;
            }
        }

        public void Acao_ChecarTipoOperacaoAritmetica()
        {
            if (_tiposExpressao.Count < 2) return;

            string tipo2 = _tiposExpressao.Pop();
            string tipo1 = _tiposExpressao.Pop();

            if (tipo1 == "string" || tipo2 == "string" || tipo1 == "indefinido" || tipo2 == "indefinido")
            {
                RegistrarErro($"Operação aritmética inválida entre os tipos '{tipo1}' e '{tipo2}'.", 0);
                _tiposExpressao.Push("indefinido");
            }
            else if (tipo1 == "real" || tipo2 == "real")
            {
                _tiposExpressao.Push("real");
            }
            else
            {
                _tiposExpressao.Push("integer");
            }
        }

        private void RegistrarErro(string mensagem, int linha)
        {
            string erroCompleto = $"Erro Semântico (Linha {linha}): {mensagem}";

            if (!ErrosSemanticos.Contains(erroCompleto))
                ErrosSemanticos.Add(erroCompleto);
        }

        private string MapearTokenParaTerminal(TokenInfo token)
        {
            if (token.Codigo == 16) return "ident";
            if (token.Codigo == 12) return "nint";
            if (token.Codigo == 11) return "nreal";
            if (token.Codigo == 13) return "literal";
            return token.Lexema;
        }
        
        public void Acao_InserirProcedure(TokenInfo identificador)
        {
            var simbolo = new Simbolo(identificador.Lexema, CategoriaSimbolo.PROCEDIMENTO, "procedure", Tabela.NivelAtual);

            if (!Tabela.Inserir(simbolo))
                RegistrarErro($"Identificador '{identificador.Lexema}' já declarado neste escopo.", identificador.Linha);
        }
    }

    public class TabelaDeSimbolos
    {
        private readonly Stack<Simbolo> _pilha = new Stack<Simbolo>();
        private int _nivelAtual = 0;
        public int NivelAtual => _nivelAtual;

        public void IniciarEscopo()
        {
            _nivelAtual++;
            Console.WriteLine($"\n--- Tabela de Símbolos: Entrando no Nível {_nivelAtual} ---");
        }

        public void FinalizarEscopo()
        {
            Console.WriteLine($"\n--- Tabela de Símbolos: Saindo do Nível {_nivelAtual} ---");
            var simbolosParaManter = _pilha.Where(s => s.Nivel < _nivelAtual).Reverse().ToList();
            _pilha.Clear();
            foreach (var s in simbolosParaManter)
            {
                _pilha.Push(s);
            }
            _nivelAtual--;
            ImprimirTabela("Após Finalizar Escopo");
        }

        public bool Inserir(Simbolo simbolo)
        {
            if (_pilha.Any(s => s.Lexema == simbolo.Lexema && s.Nivel == _nivelAtual)) return false;

            simbolo.Nivel = _nivelAtual;
            _pilha.Push(simbolo);

            ImprimirTabela($"Após Inserir '{simbolo.Lexema}' no Nível {simbolo.Nivel}");

            return true;
        }

        public Simbolo Buscar(string lexema)
        {
            return _pilha.FirstOrDefault(s => s.Lexema == lexema) ?? new Simbolo(lexema, CategoriaSimbolo.VARIAVEL, "indefinido", 0);
        }

        public void ImprimirTabela(string momento)
        {
            Console.WriteLine($"\n--- Tabela de Símbolos ({momento}) ---");
            Console.WriteLine("----------------------------------------------------------------------");
            Console.WriteLine("| Lexema          | Categoria       | Tipo       | Nível | Valor      |");
            Console.WriteLine("----------------------------------------------------------------------");
            if (!_pilha.Any())
            {
                Console.WriteLine("| (vazia)                                                            |");
            }
            else
            {
                foreach (var s in _pilha.Reverse())
                {
                    Console.WriteLine(s.ToString());
                }
            }
            Console.WriteLine("----------------------------------------------------------------------\n");
        }
    }
    
    public enum CategoriaSimbolo
    {
        VARIAVEL,
        CONSTANTE,
        PROCEDIMENTO
    }

    public class Simbolo
    {
        public string Lexema { get; set; }
        public CategoriaSimbolo Categoria { get; set; }
        public string Tipo { get; set; }
        public int Nivel { get; set; }
        public object? Valor { get; set; }

        public Simbolo(string lexema, CategoriaSimbolo categoria, string tipo, int nivel, object valor = null)
        {
            Lexema = lexema;
            Categoria = categoria;
            Tipo = tipo;
            Nivel = nivel;
            Valor = valor;
        }

        public override string ToString()
        {
            return $"| {Lexema,-15} | {Categoria,-15} | {Tipo,-10} | {Nivel,-5} | {Valor,-10} |";
        }
    }
}