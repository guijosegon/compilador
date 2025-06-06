namespace CompiladorParaConsole
{
    public class AnalisadorSintatico
    {
        private List<TokenInfo> tokens;
        private int tokenIndex;
        private Stack<string> pilha;
        private Dictionary<(string, string), string[]> tabelaParsing; // (Não-Terminal, Terminal) -> Produção
        private List<string> errosSintaticos;

        // Conjuntos de terminais e não-terminais (baseado no manual)
        private HashSet<string> terminais;
        private HashSet<string> naoTerminais;

        // Constantes para nomes de não-terminais e terminais para evitar erros de digitação
        private const string T_PROGRAM = "program";
        private const string T_IDENT = "ident";
        private const string T_SEMICOLON = ";";
        private const string T_CONST = "const";
        private const string T_EQ = "=";
        private const string T_NINT = "nint";
        private const string T_VAR = "var";
        private const string T_COLON = ":";
        private const string T_INTEGER = "integer";
        private const string T_REAL = "real";
        private const string T_STRING = "string";
        private const string T_COMMA = ",";
        private const string T_PROCEDURE = "procedure";
        private const string T_LPAREN = "(";
        private const string T_RPAREN = ")";
        private const string T_BEGIN = "begin";
        private const string T_END = "end";
        private const string T_DOT = ".";
        private const string T_PRINT = "print";
        private const string T_LBRACE = "{";
        private const string T_RBRACE = "}";
        private const string T_IF = "if";
        private const string T_THEN = "then";
        private const string T_ELSE = "else";
        private const string T_FOR = "for";
        private const string T_ASSIGN = ":=";
        private const string T_TO = "to";
        private const string T_DO = "do";
        private const string T_WHILE = "while";
        private const string T_READ = "read";
        private const string T_NE = "<>";
        private const string T_LT = "<";
        private const string T_GT = ">";
        private const string T_LE = "<=";
        private const string T_GE = ">=";
        private const string T_PLUS = "+";
        private const string T_MINUS = "-";
        private const string T_MULT = "*";
        private const string T_DIV = "/";
        private const string T_NREAL = "nreal";
        private const string T_LITERAL = "literal";
        private const string T_EOF = "$";
        private const string T_EPSILON = "î";

        private const string NT_PROGRAMA = "PROGRAMA";
        private const string NT_DECLARACOES = "DECLARACOES";
        private const string NT_CONST_SEC = "CONST_SEC";
        private const string NT_DEF_CONST_LIST = "DEF_CONST_LIST";
        private const string NT_MORE_CONST = "MORE_CONST";
        private const string NT_VAR_SEC = "VAR_SEC";
        private const string NT_DEF_VAR_LIST = "DEF_VAR_LIST";
        private const string NT_LISTAVARIAVEIS = "LISTAVARIAVEIS";
        private const string NT_LIDENT = "LIDENT";
        private const string NT_TIPO = "TIPO";
        private const string NT_PROC_SEC = "PROC_SEC";
        private const string NT_PROC_LIST = "PROC_LIST";
        private const string NT_PROC_LIST_TAIL = "PROC_LIST_TAIL";
        private const string NT_PARAMETROS = "PARÂMETROS";
        private const string NT_LISTAPARAM = "LISTAPARAM";
        private const string NT_PARAM_REST = "PARAM_REST";
        private const string NT_BLOCO = "BLOCO";
        private const string NT_COMANDOS = "COMANDOS";
        private const string NT_CMD_REST = "CMD_REST";
        private const string NT_COMANDO = "COMANDO";
        private const string NT_CMD_IDENT_REST = "CMD_IDENT_REST";
        private const string NT_CHAMADAPROC = "CHAMADAPROC";
        private const string NT_PARAMETROSCHAMADA = "PARAMETROSCHAMADA";
        private const string NT_PARAM_CHAMADA_REST = "PARAM_CHAMADA_REST";
        private const string NT_ELSEOPC = "ELSEOPC";
        private const string NT_ITEMSAIDA = "ITEMSAIDA";
        private const string NT_REPITEM = "REPITEM";
        private const string NT_EXPRELACIONAL = "EXPRELACIONAL";
        private const string NT_OPREL = "OPREL";
        private const string NT_EXPRESSAO = "EXPRESSAO";
        private const string NT_EXPR_PRIME = "EXPR'";
        private const string NT_TERMO = "TERMO";
        private const string NT_TER_PRIME = "TER'";
        private const string NT_FATOR = "FATOR";
        private const string NT_VAR_SEC_LOCAL = "VAR_SEC_LOCAL";


        public AnalisadorSintatico()
        {
            pilha = new Stack<string>();
            errosSintaticos = new List<string>();
            InicializarConjuntos();
            InicializarTabelaParsing(); // Chama a inicialização da tabela
        }

        private void InicializarConjuntos()
        {
            // Baseado nos tokens e regras do manual
            terminais = new HashSet<string>
            {
                T_PROGRAM, T_IDENT, T_SEMICOLON, T_CONST, T_EQ, T_NINT, T_VAR, T_COLON, T_INTEGER, T_REAL, T_STRING,
                T_COMMA, T_PROCEDURE, T_LPAREN, T_RPAREN, T_BEGIN, T_END, T_DOT, T_PRINT, T_LBRACE, T_RBRACE,
                T_IF, T_THEN, T_ELSE, T_FOR, T_ASSIGN, T_TO, T_DO, T_WHILE, T_READ, T_NE, T_LT, T_GT, T_LE, T_GE,
                T_PLUS, T_MINUS, T_MULT, T_DIV, T_NREAL, T_LITERAL, T_EOF
            };

            // Baseado nas regras do manual (usando constantes)
            naoTerminais = new HashSet<string>
            {
                NT_PROGRAMA, NT_DECLARACOES, NT_CONST_SEC, NT_DEF_CONST_LIST, NT_MORE_CONST, NT_VAR_SEC,
                NT_DEF_VAR_LIST, NT_LISTAVARIAVEIS, NT_LIDENT, NT_TIPO, NT_PROC_SEC, NT_PROC_LIST,
                NT_PROC_LIST_TAIL, NT_PARAMETROS, NT_LISTAPARAM, NT_PARAM_REST, NT_BLOCO, NT_COMANDOS,
                NT_CMD_REST, NT_COMANDO, NT_CMD_IDENT_REST, NT_CHAMADAPROC, NT_PARAMETROSCHAMADA,
                NT_PARAM_CHAMADA_REST, NT_ELSEOPC, NT_ITEMSAIDA, NT_REPITEM, NT_EXPRELACIONAL, NT_OPREL,
                NT_EXPRESSAO, NT_EXPR_PRIME, NT_TERMO, NT_TER_PRIME, NT_FATOR, NT_VAR_SEC_LOCAL
            };
        }

        private void AddEntry(string nonTerminal, string terminal, params string[] production)
        {
            if (!naoTerminais.Contains(nonTerminal))
                throw new ArgumentException($"Erro interno: Não-terminal desconhecido 	'{nonTerminal}	'");
            if (!terminais.Contains(terminal) && terminal != T_EPSILON) // Epsilon não está no conjunto de terminais
                throw new ArgumentException($"Erro interno: Terminal desconhecido 	'{terminal}	'");

            tabelaParsing.Add((nonTerminal, terminal), production);
        }

        private void InicializarTabelaParsing()
        {
            tabelaParsing = new Dictionary<(string, string), string[]>();

            // VAR_SEC_LOCAL -> var DEF_VAR_LIST VAR_SEC_LOCAL
            AddEntry(NT_VAR_SEC_LOCAL, T_VAR, T_VAR, NT_DEF_VAR_LIST, NT_VAR_SEC_LOCAL);
            // VAR_SEC_LOCAL -> î
            AddEntry(NT_VAR_SEC_LOCAL, T_BEGIN, T_EPSILON);

            // Regra 1: PROGRAMA -> program ident ; DECLARACOES BLOCO .
            AddEntry(NT_PROGRAMA, T_PROGRAM, T_PROGRAM, T_IDENT, T_SEMICOLON, NT_DECLARACOES, NT_BLOCO, T_DOT);

            // Regra 2: DECLARACOES -> CONST_SEC VAR_SEC PROC_SEC
            // FIRST(CONST_SEC) = {const, î}, FIRST(VAR_SEC) = {var, î}, FIRST(PROC_SEC) = {procedure, î}
            // FIRST(DECLARACOES) = FIRST(CONST_SEC) U FIRST(VAR_SEC) U FIRST(PROC_SEC) = {const, var, procedure, î}
            // FOLLOW(DECLARACOES) = {begin}
            AddEntry(NT_DECLARACOES, T_CONST, NT_CONST_SEC, NT_VAR_SEC, NT_PROC_SEC);
            AddEntry(NT_DECLARACOES, T_VAR, NT_CONST_SEC, NT_VAR_SEC, NT_PROC_SEC); // î for CONST_SEC
            AddEntry(NT_DECLARACOES, T_PROCEDURE, NT_CONST_SEC, NT_VAR_SEC, NT_PROC_SEC); // î for CONST_SEC, VAR_SEC
            AddEntry(NT_DECLARACOES, T_BEGIN, NT_CONST_SEC, NT_VAR_SEC, NT_PROC_SEC); // î for CONST_SEC, VAR_SEC, PROC_SEC (FOLLOW)

            // Regra 3: CONST_SEC -> const DEF_CONST_LIST | î
            // FIRST(const DEF_CONST_LIST) = {const}
            // FOLLOW(CONST_SEC) = {var, procedure, begin}
            AddEntry(NT_CONST_SEC, T_CONST, T_CONST, NT_DEF_CONST_LIST);
            AddEntry(NT_CONST_SEC, T_VAR, T_EPSILON);
            AddEntry(NT_CONST_SEC, T_PROCEDURE, T_EPSILON);
            AddEntry(NT_CONST_SEC, T_BEGIN, T_EPSILON);

            // Regra 4: DEF_CONST_LIST -> ident = nint ; MORE_CONST
            AddEntry(NT_DEF_CONST_LIST, T_IDENT, T_IDENT, T_EQ, T_NINT, T_SEMICOLON, NT_MORE_CONST);

            // Regra 5: MORE_CONST -> ident = nint ; MORE_CONST | î
            // FIRST(ident...) = {ident}
            // FOLLOW(MORE_CONST) = FOLLOW(CONST_SEC) = {var, procedure, begin}
            AddEntry(NT_MORE_CONST, T_IDENT, T_IDENT, T_EQ, T_NINT, T_SEMICOLON, NT_MORE_CONST);
            AddEntry(NT_MORE_CONST, T_VAR, T_EPSILON);
            AddEntry(NT_MORE_CONST, T_PROCEDURE, T_EPSILON);
            AddEntry(NT_MORE_CONST, T_BEGIN, T_EPSILON);

            // Regra 6: VAR_SEC -> var DEF_VAR_LIST  NT_VAR_SEC| î
            // FIRST(var...) = {var}
            // FOLLOW(VAR_SEC) = {procedure, begin}
            AddEntry(NT_VAR_SEC, T_VAR, T_VAR, NT_DEF_VAR_LIST, NT_VAR_SEC);
            AddEntry(NT_VAR_SEC, T_PROCEDURE, T_EPSILON);
            AddEntry(NT_VAR_SEC, T_BEGIN, T_EPSILON);

            // Regra 7: DEF_VAR_LIST -> LISTAVARIAVEIS : TIPO ;
            AddEntry(NT_DEF_VAR_LIST, T_IDENT, NT_LISTAVARIAVEIS, T_COLON, NT_TIPO, T_SEMICOLON);

            // Regra 9: LISTAVARIAVEIS -> ident LIDENT
            AddEntry(NT_LISTAVARIAVEIS, T_IDENT, T_IDENT, NT_LIDENT);

            // Regra 10: LIDENT -> , ident LIDENT | î
            // FIRST(,) = {,}
            // FOLLOW(LIDENT) = { : }
            AddEntry(NT_LIDENT, T_COMMA, T_COMMA, T_IDENT, NT_LIDENT);
            AddEntry(NT_LIDENT, T_COLON, T_EPSILON);

            // Regra 11: TIPO -> integer | real | string
            AddEntry(NT_TIPO, T_INTEGER, T_INTEGER);
            AddEntry(NT_TIPO, T_REAL, T_REAL);
            AddEntry(NT_TIPO, T_STRING, T_STRING);

            // Regra 12: PROC_SEC -> PROC_LIST | î
            // FIRST(PROC_LIST) = {procedure}
            // FOLLOW(PROC_SEC) = {begin}
            AddEntry(NT_PROC_SEC, T_PROCEDURE, NT_PROC_LIST);
            AddEntry(NT_PROC_SEC, T_BEGIN, T_EPSILON);

            // Regra 13: PROC_LIST -> procedure ident PARÂMETROS ; BLOCO ; PROC_LIST_TAIL
            AddEntry(NT_PROC_LIST, T_PROCEDURE, T_PROCEDURE, T_IDENT, NT_PARAMETROS, T_SEMICOLON, NT_BLOCO, T_SEMICOLON, NT_PROC_LIST_TAIL);

            // Regra 14: PROC_LIST_TAIL -> procedure ident PARÂMETROS ; BLOCO ; PROC_LIST_TAIL | î
            // FIRST(procedure...) = {procedure}
            // FOLLOW(PROC_LIST_TAIL) = FOLLOW(PROC_SEC) = {begin}
            AddEntry(NT_PROC_LIST_TAIL, T_PROCEDURE, T_PROCEDURE, T_IDENT, NT_PARAMETROS, T_SEMICOLON, NT_BLOCO, T_SEMICOLON, NT_PROC_LIST_TAIL);
            AddEntry(NT_PROC_LIST_TAIL, T_BEGIN, T_EPSILON);

            // Regra 15: PARÂMETROS -> ( LISTAPARAM ) | î
            // FIRST(() = { ( }
            // FOLLOW(PARÂMETROS) = { ; }
            AddEntry(NT_PARAMETROS, T_LPAREN, T_LPAREN, NT_LISTAPARAM, T_RPAREN);
            AddEntry(NT_PARAMETROS, T_SEMICOLON, T_EPSILON);

            // Regra 16: LISTAPARAM -> ident : TIPO PARAM_REST | î
            // FIRST(ident...) = {ident}
            // FOLLOW(LISTAPARAM) = { ) }
            AddEntry(NT_LISTAPARAM, T_IDENT, T_IDENT, T_COLON, NT_TIPO, NT_PARAM_REST);
            AddEntry(NT_LISTAPARAM, T_RPAREN, T_EPSILON);

            // Regra 17: PARAM_REST -> , ident : TIPO PARAM_REST | î
            // FIRST(,) = {,}
            // FOLLOW(PARAM_REST) = { ) }
            AddEntry(NT_PARAM_REST, T_COMMA, T_COMMA, T_IDENT, T_COLON, NT_TIPO, NT_PARAM_REST);
            AddEntry(NT_PARAM_REST, T_RPAREN, T_EPSILON);

            // Regra 18 (nova): BLOCO -> VAR_SEC_LOCAL begin COMANDOS end
            AddEntry(NT_BLOCO, T_VAR,   NT_VAR_SEC_LOCAL, T_BEGIN, NT_COMANDOS, T_END);
            AddEntry(NT_BLOCO, T_BEGIN, NT_VAR_SEC_LOCAL, T_BEGIN, NT_COMANDOS, T_END);

            // Regra 19: COMANDOS -> COMANDO CMD_REST | î
            // FIRST(COMANDO) = {print, if, ident, for, while, read}
            // FOLLOW(COMANDOS) = {end}
            AddEntry(NT_COMANDOS, T_PRINT, NT_COMANDO, NT_CMD_REST);
            AddEntry(NT_COMANDOS, T_IF, NT_COMANDO, NT_CMD_REST);
            AddEntry(NT_COMANDOS, T_IDENT, NT_COMANDO, NT_CMD_REST);
            AddEntry(NT_COMANDOS, T_FOR, NT_COMANDO, NT_CMD_REST);
            AddEntry(NT_COMANDOS, T_WHILE, NT_COMANDO, NT_CMD_REST);
            AddEntry(NT_COMANDOS, T_READ, NT_COMANDO, NT_CMD_REST);
            AddEntry(NT_COMANDOS, T_END, T_EPSILON);

            // Regra 20: CMD_REST -> ; COMANDO CMD_REST | î
            // FIRST(;) = {;}
            // FOLLOW(CMD_REST) = {end}
            AddEntry(NT_CMD_REST, T_SEMICOLON, T_SEMICOLON, NT_COMANDO, NT_CMD_REST);
            AddEntry(NT_CMD_REST, T_END, T_EPSILON);

            // Regra 21: COMANDO -> print { ITEMSAIDA REPITEM }
            AddEntry(NT_COMANDO, T_PRINT, T_PRINT, T_LBRACE, NT_ITEMSAIDA, NT_REPITEM, T_RBRACE);

            // Regra 22: COMANDO -> if EXPRELACIONAL then BLOCO ELSEOPC
            AddEntry(NT_COMANDO, T_IF, T_IF, NT_EXPRELACIONAL, T_THEN, NT_BLOCO, NT_ELSEOPC);

            // Regra 23: COMANDO -> ident CMD_IDENT_REST
            AddEntry(NT_COMANDO, T_IDENT, T_IDENT, NT_CMD_IDENT_REST);

            // Regra 23b: COMANDO -> ε  (quando vier 'end')
            AddEntry(NT_COMANDO, T_END, T_EPSILON);

            // Regra 24: CMD_IDENT_REST -> := EXPRESSAO | CHAMADAPROC
            // FIRST(:= EXPRESSAO) = { := }
            // FIRST(CHAMADAPROC) = { (, î }
            // FOLLOW(CMD_IDENT_REST) = FOLLOW(COMANDO) = { ;, end }
            AddEntry(NT_CMD_IDENT_REST, T_ASSIGN, T_ASSIGN, NT_EXPRESSAO);
            AddEntry(NT_CMD_IDENT_REST, T_LPAREN, NT_CHAMADAPROC); // Chamada com parâmetros
            // Se î em FIRST(CHAMADAPROC), usar FOLLOW(CMD_IDENT_REST)
            AddEntry(NT_CMD_IDENT_REST, T_SEMICOLON, NT_CHAMADAPROC); // Chamada sem parâmetros (epsilon)
            AddEntry(NT_CMD_IDENT_REST, T_END, NT_CHAMADAPROC); // Chamada sem parâmetros (epsilon)

            // Regra 25: CHAMADAPROC -> ( PARAMETROSCHAMADA ) | î
            // FIRST(() = { ( }
            // FOLLOW(CHAMADAPROC) = FOLLOW(CMD_IDENT_REST) = { ;, end }
            AddEntry(NT_CHAMADAPROC, T_LPAREN, T_LPAREN, NT_PARAMETROSCHAMADA, T_RPAREN);
            AddEntry(NT_CHAMADAPROC, T_SEMICOLON, T_EPSILON);
            AddEntry(NT_CHAMADAPROC, T_END, T_EPSILON);

            // Regra 26: PARAMETROSCHAMADA -> EXPRESSAO PARAM_CHAMADA_REST | î
            // FIRST(EXPRESSAO) = { ident, nint, nreal, literal, ( }
            // FOLLOW(PARAMETROSCHAMADA) = { ) }
            AddEntry(NT_PARAMETROSCHAMADA, T_IDENT, NT_EXPRESSAO, NT_PARAM_CHAMADA_REST);
            AddEntry(NT_PARAMETROSCHAMADA, T_NINT, NT_EXPRESSAO, NT_PARAM_CHAMADA_REST);
            AddEntry(NT_PARAMETROSCHAMADA, T_NREAL, NT_EXPRESSAO, NT_PARAM_CHAMADA_REST);
            AddEntry(NT_PARAMETROSCHAMADA, T_LITERAL, NT_EXPRESSAO, NT_PARAM_CHAMADA_REST);
            AddEntry(NT_PARAMETROSCHAMADA, T_LPAREN, NT_EXPRESSAO, NT_PARAM_CHAMADA_REST);
            AddEntry(NT_PARAMETROSCHAMADA, T_RPAREN, T_EPSILON);

            // Regra 27: PARAM_CHAMADA_REST -> , EXPRESSAO PARAM_CHAMADA_REST | î
            // FIRST(,) = {,}
            // FOLLOW(PARAM_CHAMADA_REST) = { ) }
            AddEntry(NT_PARAM_CHAMADA_REST, T_COMMA, T_COMMA, NT_EXPRESSAO, NT_PARAM_CHAMADA_REST);
            AddEntry(NT_PARAM_CHAMADA_REST, T_RPAREN, T_EPSILON);

            // Regra 28: COMANDO -> for ident := EXPRESSAO to EXPRESSAO do BLOCO
            AddEntry(NT_COMANDO, T_FOR, T_FOR, T_IDENT, T_ASSIGN, NT_EXPRESSAO, T_TO, NT_EXPRESSAO, T_DO, NT_BLOCO);

            // Regra 29: COMANDO -> while EXPRELACIONAL do BLOCO
            AddEntry(NT_COMANDO, T_WHILE, T_WHILE, NT_EXPRELACIONAL, T_DO, NT_BLOCO);

            // Regra 30: COMANDO -> read ( ident )
            AddEntry(NT_COMANDO, T_READ, T_READ, T_LPAREN, T_IDENT, T_RPAREN);

            // Regra 31: ELSEOPC -> else BLOCO | î
            // FIRST(else) = {else}
            // FOLLOW(ELSEOPC) = FOLLOW(COMANDO) = { ;, end }
            AddEntry(NT_ELSEOPC, T_ELSE, T_ELSE, NT_BLOCO);
            AddEntry(NT_ELSEOPC, T_SEMICOLON, T_EPSILON);
            AddEntry(NT_ELSEOPC, T_END, T_EPSILON);

            // Regra 32: ITEMSAIDA -> EXPRESSAO
            // FIRST(EXPRESSAO) = { ident, nint, nreal, literal, ( }
            AddEntry(NT_ITEMSAIDA, T_IDENT, NT_EXPRESSAO);
            AddEntry(NT_ITEMSAIDA, T_NINT, NT_EXPRESSAO);
            AddEntry(NT_ITEMSAIDA, T_NREAL, NT_EXPRESSAO);
            AddEntry(NT_ITEMSAIDA, T_LITERAL, NT_EXPRESSAO);
            AddEntry(NT_ITEMSAIDA, T_LPAREN, NT_EXPRESSAO);

            // Regra 33: REPITEM -> , ITEMSAIDA REPITEM | î
            // FIRST(,) = {,}
            // FOLLOW(REPITEM) = { } }
            AddEntry(NT_REPITEM, T_COMMA, T_COMMA, NT_ITEMSAIDA, NT_REPITEM);
            AddEntry(NT_REPITEM, T_RBRACE, T_EPSILON);

            // Regra 34: EXPRELACIONAL -> EXPRESSAO OPREL EXPRESSAO
            // FIRST(EXPRESSAO) = { ident, nint, nreal, literal, ( }
            AddEntry(NT_EXPRELACIONAL, T_IDENT, NT_EXPRESSAO, NT_OPREL, NT_EXPRESSAO);
            AddEntry(NT_EXPRELACIONAL, T_NINT, NT_EXPRESSAO, NT_OPREL, NT_EXPRESSAO);
            AddEntry(NT_EXPRELACIONAL, T_NREAL, NT_EXPRESSAO, NT_OPREL, NT_EXPRESSAO);
            AddEntry(NT_EXPRELACIONAL, T_LITERAL, NT_EXPRESSAO, NT_OPREL, NT_EXPRESSAO);
            AddEntry(NT_EXPRELACIONAL, T_LPAREN, NT_EXPRESSAO, NT_OPREL, NT_EXPRESSAO);

            // Regra 35: OPREL -> = | <> | < | > | <= | >=
            AddEntry(NT_OPREL, T_EQ, T_EQ);
            AddEntry(NT_OPREL, T_NE, T_NE);
            AddEntry(NT_OPREL, T_LT, T_LT);
            AddEntry(NT_OPREL, T_GT, T_GT);
            AddEntry(NT_OPREL, T_LE, T_LE);
            AddEntry(NT_OPREL, T_GE, T_GE);

            // Regra 36: EXPRESSAO -> TERMO EXPR'
            // FIRST(TERMO) = { ident, nint, nreal, literal, ( }
            AddEntry(NT_EXPRESSAO, T_IDENT, NT_TERMO, NT_EXPR_PRIME);
            AddEntry(NT_EXPRESSAO, T_NINT, NT_TERMO, NT_EXPR_PRIME);
            AddEntry(NT_EXPRESSAO, T_NREAL, NT_TERMO, NT_EXPR_PRIME);
            AddEntry(NT_EXPRESSAO, T_LITERAL, NT_TERMO, NT_EXPR_PRIME);
            AddEntry(NT_EXPRESSAO, T_LPAREN, NT_TERMO, NT_EXPR_PRIME);

            // Regra 37: EXPR' -> + TERMO EXPR' | - TERMO EXPR' | î
            // FIRST(+) = {+}, FIRST(-) = {-}
            // FOLLOW(EXPR') = { =, <>, <, >, <=, >=, to, do, ;, ,, ), } }
            AddEntry(NT_EXPR_PRIME, T_PLUS, T_PLUS, NT_TERMO, NT_EXPR_PRIME);
            AddEntry(NT_EXPR_PRIME, T_MINUS, T_MINUS, NT_TERMO, NT_EXPR_PRIME);
            AddEntry(NT_EXPR_PRIME, T_EQ, T_EPSILON);
            AddEntry(NT_EXPR_PRIME, T_NE, T_EPSILON);
            AddEntry(NT_EXPR_PRIME, T_LT, T_EPSILON);
            AddEntry(NT_EXPR_PRIME, T_GT, T_EPSILON);
            AddEntry(NT_EXPR_PRIME, T_LE, T_EPSILON);
            AddEntry(NT_EXPR_PRIME, T_GE, T_EPSILON);
            AddEntry(NT_EXPR_PRIME, T_TO, T_EPSILON);
            AddEntry(NT_EXPR_PRIME, T_DO, T_EPSILON);
            AddEntry(NT_EXPR_PRIME, T_SEMICOLON, T_EPSILON);
            AddEntry(NT_EXPR_PRIME, T_COMMA, T_EPSILON);
            AddEntry(NT_EXPR_PRIME, T_RPAREN, T_EPSILON);
            AddEntry(NT_EXPR_PRIME, T_RBRACE, T_EPSILON);
            AddEntry(NT_EXPR_PRIME, T_THEN, T_EPSILON); // Added based on EXPRELACIONAL context
            AddEntry(NT_EXPR_PRIME, T_END, T_EPSILON); // Added based on context

            // Regra 38: TERMO -> FATOR TER'
            // FIRST(FATOR) = { ident, nint, nreal, literal, ( }
            AddEntry(NT_TERMO, T_IDENT, NT_FATOR, NT_TER_PRIME);
            AddEntry(NT_TERMO, T_NINT, NT_FATOR, NT_TER_PRIME);
            AddEntry(NT_TERMO, T_NREAL, NT_FATOR, NT_TER_PRIME);
            AddEntry(NT_TERMO, T_LITERAL, NT_FATOR, NT_TER_PRIME);
            AddEntry(NT_TERMO, T_LPAREN, NT_FATOR, NT_TER_PRIME);

            // Regra 39: TER' -> * FATOR TER' | / FATOR TER' | î
            // FIRST(*) = {*}, FIRST(/) = {/}
            // FOLLOW(TER') = FOLLOW(TERMO) = { +, -, =, <>, <, >, <=, >=, to, do, ;, ,, ), } }
            AddEntry(NT_TER_PRIME, T_MULT, T_MULT, NT_FATOR, NT_TER_PRIME);
            AddEntry(NT_TER_PRIME, T_DIV, T_DIV, NT_FATOR, NT_TER_PRIME);
            // Epsilon entries based on FOLLOW(TER')
            AddEntry(NT_TER_PRIME, T_PLUS, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_MINUS, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_EQ, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_NE, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_LT, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_GT, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_LE, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_GE, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_TO, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_DO, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_SEMICOLON, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_COMMA, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_RPAREN, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_RBRACE, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_THEN, T_EPSILON); // Added based on EXPRELACIONAL context
            AddEntry(NT_TER_PRIME, T_END, T_EPSILON); // Added based on context

            // Regra 40: FATOR -> ident | nint | nreal | literal | ( EXPRESSAO )
            AddEntry(NT_FATOR, T_IDENT, T_IDENT);
            AddEntry(NT_FATOR, T_NINT, T_NINT);
            AddEntry(NT_FATOR, T_NREAL, T_NREAL);
            AddEntry(NT_FATOR, T_LITERAL, T_LITERAL);
            AddEntry(NT_FATOR, T_LPAREN, T_LPAREN, NT_EXPRESSAO, T_RPAREN);
        }

        public bool Analisar(List<TokenInfo> tokensEntrada)
        {
            this.tokens = tokensEntrada;
            // Adiciona marcador de fim de entrada ($) se não existir
            if (!this.tokens.Any() || this.tokens.Last().Lexema != T_EOF)
            {
                this.tokens.Add(new TokenInfo(0, T_EOF, tokens.LastOrDefault()?.Linha ?? 1));
            }

            this.tokenIndex = 0;
            this.errosSintaticos.Clear();

            pilha.Clear();
            pilha.Push(T_EOF); // Marcador de fundo da pilha
            pilha.Push(NT_PROGRAMA); // Símbolo inicial da gramática

            Console.WriteLine("\n--- Análise Sintática ---");
            MostrarPilha("Inicial");

            do
            {
                string topo = pilha.Peek();
                TokenInfo tokenAtual = tokenIndex < tokens.Count ? tokens[tokenIndex] : new TokenInfo(0, "$", tokens.LastOrDefault()?.Linha ?? 1);
                string terminalAtual = MapearTokenParaTerminal(tokenAtual);

                Console.WriteLine($"Pilha: [ {string.Join(", ", pilha.Reverse())} ] | Token Atual: {terminalAtual} (	'{tokenAtual.Lexema}		' Linha: {tokenAtual.Linha})");

                if (terminais.Contains(topo))
                {
                    if (topo == terminalAtual)
                    {
                        pilha.Pop();
                        tokenIndex++;
                        // MostrarPilha("Match"); // Muita verbosidade
                    }
                    else
                    {
                        RegistrarErroMatch(topo, tokenAtual);
                        // Recuperação de erro (Pânico): Avança token até encontrar algo esperado ou sincronizador
                        // Tentativa simples: avançar token
                        tokenIndex++;
                        if (terminalAtual == T_EOF) break; // Evita loop infinito no fim
                    }
                }
                else if (naoTerminais.Contains(topo))
                {
                    if (tabelaParsing.ContainsKey((topo, terminalAtual)))
                    {
                        pilha.Pop();
                        string[] producao = tabelaParsing[(topo, terminalAtual)];
                        // Empilha a produção em ordem reversa
                        for (int i = producao.Length - 1; i >= 0; i--)
                        {
                            if (producao[i] != T_EPSILON) // î representa vazio (epsilon)
                            {
                                pilha.Push(producao[i]);
                            }
                        }
                        //MostrarPilha("Produção"); // Muita verbosidade
                    }
                    else
                    {
                        RegistrarErroParsing(topo, tokenAtual);
                        // Recuperação de erro (Pânico):
                        // Opção 1: Descartar token atual
                        tokenIndex++;
                        Console.WriteLine($"[RECUPERAÇÃO] Descartando token 	'{terminalAtual}	' e continuando.");
                        if (terminalAtual == T_EOF) break; // Evita loop infinito no fim

                        // Opção 2: Desempilhar não-terminal (pode ser perigoso)
                        // pilha.Pop();
                        // Console.WriteLine($"[RECUPERAÇÃO] Desempilhando 	'{topo}	' e continuando.");
                    }
                }
                else
                {
                    // Símbolo inesperado na pilha (erro interno)
                    Console.WriteLine($"[ERRO INTERNO] Símbolo inesperado na pilha: {topo}");
                    errosSintaticos.Add($"Erro interno: Símbolo inesperado na pilha 	'{topo}	'");
                    return false; // Aborta análise
                }

            } while (pilha.Peek() != T_EOF);

            if (tokenIndex < tokens.Count && tokens[tokenIndex].Lexema == T_EOF)
            {
                if (errosSintaticos.Count == 0)
                {
                    Console.WriteLine("\nAnálise Sintática concluída com sucesso!");
                    return true;
                }
                else
                {
                    Console.WriteLine("\nAnálise Sintática concluída com erros.");
                    MostrarErros();
                    return false;
                }
            }
            else
            {
                // A pilha está vazia ($ no topo), mas ainda há tokens na entrada
                Console.WriteLine("\nAnálise Sintática concluída com erros.");

                if (tokenIndex < tokens.Count)
                    errosSintaticos.Add($"Erro: Tokens restantes após o fim esperado do programa na linha {tokens[tokenIndex].Linha}. Token inesperado: '{tokens[tokenIndex].Lexema}'");
                else
                    errosSintaticos.Add("Erro: Tokens restantes após o fim esperado do programa, mas não foi possível identificar o token pois o índice excedeu o tamanho da lista.");

                MostrarErros();
                return false;
            }
        }

        private string MapearTokenParaTerminal(TokenInfo token)
        {
            // Mapeia o código/lexema do token para o terminal correspondente na gramática
            if (token.Lexema == T_EOF) return T_EOF;
            if (token.Codigo == 16) return T_IDENT; // ident
            if (token.Codigo == 12) return T_NINT; // nint
            if (token.Codigo == 11) return T_NREAL; // nreal
            if (token.Codigo == 13) return T_LITERAL; // literal (string)
            // Para palavras reservadas e símbolos, o lexema geralmente corresponde ao terminal
            if (terminais.Contains(token.Lexema))
            {
                return token.Lexema;
            }

            // Se não for nenhum dos anteriores, pode ser um erro léxico já tratado
            // ou um token inesperado para o sintático.
            // Retornar o próprio lexema pode causar falha na busca da tabela,
            // o que é tratado como erro sintático.
            Console.WriteLine($"[AVISO] Mapeamento: Token 	'{token.Lexema}	' (Código: {token.Codigo}) não é um terminal esperado diretamente. Usando lexema.");
            return token.Lexema;
        }

        private void MostrarPilha(string acao)
        {
            // Limitando a verbosidade, descomente se precisar depurar passo a passo
            // Console.WriteLine($"Pilha ({acao}): [ {string.Join(", ", pilha.Reverse())} ]");
        }

        private void RegistrarErroMatch(string esperado, TokenInfo recebido)
        {
            string msg = $"Erro Sintático: Esperado terminal 	'{esperado}	' mas recebeu 	'{recebido.Lexema}	' na linha {recebido.Linha}.";
            Console.WriteLine($"[ERRO] {msg}");
            if (!errosSintaticos.Contains(msg)) errosSintaticos.Add(msg);
        }

        private void RegistrarErroParsing(string naoTerminal, TokenInfo recebido)
        {
            string msg = $"Erro Sintático: Não foi possível aplicar regra para 	'{naoTerminal}	' com o token 	'{recebido.Lexema}	' na linha {recebido.Linha}. Token inesperado.";
            // Poderia listar os terminais esperados consultando a tabela para `naoTerminal`
            Console.WriteLine($"[ERRO] {msg}");
            if (!errosSintaticos.Contains(msg)) errosSintaticos.Add(msg);
        }

        public void MostrarErros()
        {
            if (errosSintaticos.Any())
            {
                Console.WriteLine("\n--- Erros Sintáticos Detectados ---");
                foreach (var erro in errosSintaticos)
                {
                    Console.WriteLine(erro);
                }
            }
        }
    }
}