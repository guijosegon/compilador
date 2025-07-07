namespace CompiladorParaConsole
{
    public class AnalisadorSintatico
    {
        private List<TokenInfo> tokens;
        private int tokenIndex;
        private Stack<string> pilha;
        private Dictionary<(string, string), string[]> tabelaParsing;
        private List<string> errosSintaticos;

        private AnalisadorSemantico analisadorSemantico;
        private TokenInfo tokenLidoAnteriormente;
        private string tipoAtual;
        private List<TokenInfo> identificadoresParaDeclarar = new List<TokenInfo>();
        private TokenInfo identificadorConstante;
        private TokenInfo identificadorProcedure;
        private List<TokenInfo> parametrosParaDeclarar = new List<TokenInfo>();

        private const string ACAO_INSERIR_CONST = "#ACAO_INSERIR_CONST";
        private const string ACAO_INSERIR_VAR_LISTA = "#ACAO_INSERIR_VAR_LISTA";
        private const string ACAO_INSERIR_PROC = "#ACAO_INSERIR_PROC";
        private const string ACAO_INSERIR_PARAM_LISTA = "#ACAO_INSERIR_PARAM_LISTA";
        private const string ACAO_VERIFICAR_DECL = "#ACAO_VERIFICAR_DECL";
        private const string ACAO_VERIFICAR_ATRIB = "#ACAO_VERIFICAR_ATRIB";
        private const string ACAO_EMPILHAR_TIPO = "#ACAO_EMPILHAR_TIPO";
        private const string ACAO_CHECAR_OP_ARIT = "#ACAO_CHECAR_OP_ARIT";
        private const string ACAO_INICIAR_ESCOPO = "#ACAO_INICIAR_ESCOPO";
        private const string ACAO_FINALIZAR_ESCOPO = "#ACAO_FINALIZAR_ESCOPO";
        private const string ACAO_GUARDAR_TIPO = "#ACAO_GUARDAR_TIPO";
        private const string ACAO_GUARDAR_IDENT_PARA_DECL = "#ACAO_GUARDAR_IDENT_PARA_DECL";
        private const string ACAO_GUARDAR_IDENT_PARA_CONST = "#ACAO_GUARDAR_IDENT_PARA_CONST";
        private const string ACAO_GUARDAR_IDENT_PARA_PROC = "#ACAO_GUARDAR_IDENT_PARA_PROC";
        private const string ACAO_GUARDAR_IDENT_PARA_PARAM = "#ACAO_GUARDAR_IDENT_PARA_PARAM";
        private const string ACAO_GUARDAR_IDENT_PARA_USO = "#ACAO_GUARDAR_IDENT_PARA_USO";

        private HashSet<string> terminais;
        private HashSet<string> naoTerminais;

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
            analisadorSemantico = new AnalisadorSemantico();
            InicializarConjuntos();
            InicializarTabelaParsing();
        }

        private void InicializarConjuntos()
        {
            terminais = new HashSet<string>
            {
                T_PROGRAM, T_IDENT, T_SEMICOLON, T_CONST, T_EQ, T_NINT, T_VAR, T_COLON, T_INTEGER, T_REAL, T_STRING,
                T_COMMA, T_PROCEDURE, T_LPAREN, T_RPAREN, T_BEGIN, T_END, T_DOT, T_PRINT, T_LBRACE, T_RBRACE,
                T_IF, T_THEN, T_ELSE, T_FOR, T_ASSIGN, T_TO, T_DO, T_WHILE, T_READ, T_NE, T_LT, T_GT, T_LE, T_GE,
                T_PLUS, T_MINUS, T_MULT, T_DIV, T_NREAL, T_LITERAL, T_EOF
            };

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
            // O bloco principal do programa é o escopo global (Nível 0), não precisa de Iniciar/FinalizarEscopo aqui.
            AddEntry(NT_PROGRAMA, T_PROGRAM, T_PROGRAM, T_IDENT, T_SEMICOLON, NT_DECLARACOES, NT_BLOCO, T_DOT);

            // Regra 2: DECLARACOES -> CONST_SEC VAR_SEC PROC_SEC
            AddEntry(NT_DECLARACOES, T_CONST, NT_CONST_SEC, NT_VAR_SEC, NT_PROC_SEC);
            AddEntry(NT_DECLARACOES, T_VAR, NT_CONST_SEC, NT_VAR_SEC, NT_PROC_SEC);
            AddEntry(NT_DECLARACOES, T_PROCEDURE, NT_CONST_SEC, NT_VAR_SEC, NT_PROC_SEC);
            AddEntry(NT_DECLARACOES, T_BEGIN, NT_CONST_SEC, NT_VAR_SEC, NT_PROC_SEC);

            // Regra 3: CONST_SEC -> const DEF_CONST_LIST | î
            AddEntry(NT_CONST_SEC, T_CONST, T_CONST, NT_DEF_CONST_LIST);
            AddEntry(NT_CONST_SEC, T_VAR, T_EPSILON);
            AddEntry(NT_CONST_SEC, T_PROCEDURE, T_EPSILON);
            AddEntry(NT_CONST_SEC, T_BEGIN, T_EPSILON);

            // Regra 4: DEF_CONST_LIST -> ident #ACAO_GUARDAR_IDENT_PARA_CONST = nint #ACAO_INSERIR_CONST ; MORE_CONST
            AddEntry(NT_DEF_CONST_LIST, T_IDENT, T_IDENT, ACAO_GUARDAR_IDENT_PARA_CONST, T_EQ, T_NINT, ACAO_INSERIR_CONST, T_SEMICOLON, NT_MORE_CONST);

            // Regra 5: MORE_CONST -> ident #ACAO_GUARDAR_IDENT_PARA_CONST = nint #ACAO_INSERIR_CONST ; MORE_CONST | î
            AddEntry(NT_MORE_CONST, T_IDENT, T_IDENT, ACAO_GUARDAR_IDENT_PARA_CONST, T_EQ, T_NINT, ACAO_INSERIR_CONST, T_SEMICOLON, NT_MORE_CONST);
            AddEntry(NT_MORE_CONST, T_VAR, T_EPSILON);
            AddEntry(NT_MORE_CONST, T_PROCEDURE, T_EPSILON);
            AddEntry(NT_MORE_CONST, T_BEGIN, T_EPSILON);

            // Regra 6: VAR_SEC -> var DEF_VAR_LIST VAR_SEC | î
            AddEntry(NT_VAR_SEC, T_VAR, T_VAR, NT_DEF_VAR_LIST, NT_VAR_SEC);
            AddEntry(NT_VAR_SEC, T_PROCEDURE, T_EPSILON);
            AddEntry(NT_VAR_SEC, T_BEGIN, T_EPSILON);

            // Regra 7: DEF_VAR_LIST -> LISTAVARIAVEIS : TIPO ; #ACAO_INSERIR_VAR_LISTA
            AddEntry(NT_DEF_VAR_LIST, T_IDENT, NT_LISTAVARIAVEIS, T_COLON, NT_TIPO, T_SEMICOLON, ACAO_INSERIR_VAR_LISTA);

            // Regra 9: LISTAVARIAVEIS -> ident #ACAO_GUARDAR_IDENT_PARA_DECL LIDENT
            AddEntry(NT_LISTAVARIAVEIS, T_IDENT, T_IDENT, ACAO_GUARDAR_IDENT_PARA_DECL, NT_LIDENT);

            // Regra 10: LIDENT -> , ident #ACAO_GUARDAR_IDENT_PARA_DECL LIDENT | î
            AddEntry(NT_LIDENT, T_COMMA, T_COMMA, T_IDENT, ACAO_GUARDAR_IDENT_PARA_DECL, NT_LIDENT);
            AddEntry(NT_LIDENT, T_COLON, T_EPSILON);

            // Regra 11: TIPO -> (integer | real | string) #ACAO_GUARDAR_TIPO
            AddEntry(NT_TIPO, T_INTEGER, T_INTEGER, ACAO_GUARDAR_TIPO);
            AddEntry(NT_TIPO, T_REAL, T_REAL, ACAO_GUARDAR_TIPO);
            AddEntry(NT_TIPO, T_STRING, T_STRING, ACAO_GUARDAR_TIPO);

            // Regra 12: PROC_SEC -> PROC_LIST | î
            AddEntry(NT_PROC_SEC, T_PROCEDURE, NT_PROC_LIST);
            AddEntry(NT_PROC_SEC, T_BEGIN, T_EPSILON);

            // Regra 13: PROC_LIST -> procedure ident #ACAO_GUARDAR_IDENT_PARA_PROC #ACAO_INSERIR_PROC #ACAO_INICIAR_ESCOPO PARÂMETROS #ACAO_INSERIR_PARAM_LISTA ; BLOCO #ACAO_FINALIZAR_ESCOPO ; PROC_LIST_TAIL
            AddEntry(NT_PROC_LIST, T_PROCEDURE, T_PROCEDURE, T_IDENT, ACAO_GUARDAR_IDENT_PARA_PROC, ACAO_INSERIR_PROC, ACAO_INICIAR_ESCOPO, NT_PARAMETROS, ACAO_INSERIR_PARAM_LISTA, T_SEMICOLON, NT_BLOCO, ACAO_FINALIZAR_ESCOPO, T_SEMICOLON, NT_PROC_LIST_TAIL);

            // Regra 14: PROC_LIST_TAIL -> procedure ident #ACAO_GUARDAR_IDENT_PARA_PROC #ACAO_INSERIR_PROC #ACAO_INICIAR_ESCOPO PARÂMETROS #ACAO_INSERIR_PARAM_LISTA ; BLOCO #ACAO_FINALIZAR_ESCOPO ; PROC_LIST_TAIL | î
            AddEntry(NT_PROC_LIST_TAIL, T_PROCEDURE, T_PROCEDURE, T_IDENT, ACAO_GUARDAR_IDENT_PARA_PROC, ACAO_INSERIR_PROC, ACAO_INICIAR_ESCOPO, NT_PARAMETROS, ACAO_INSERIR_PARAM_LISTA, T_SEMICOLON, NT_BLOCO, ACAO_FINALIZAR_ESCOPO, T_SEMICOLON, NT_PROC_LIST_TAIL);
            AddEntry(NT_PROC_LIST_TAIL, T_BEGIN, T_EPSILON);

            // Regra 15: PARÂMETROS -> ( LISTAPARAM ) | î
            AddEntry(NT_PARAMETROS, T_LPAREN, T_LPAREN, NT_LISTAPARAM, T_RPAREN);
            AddEntry(NT_PARAMETROS, T_SEMICOLON, T_EPSILON);

            // Regra 16: LISTAPARAM -> ident #ACAO_GUARDAR_IDENT_PARA_PARAM : TIPO #ACAO_GUARDAR_TIPO PARAM_REST | î
            AddEntry(NT_LISTAPARAM, T_IDENT, T_IDENT, ACAO_GUARDAR_IDENT_PARA_PARAM, T_COLON, NT_TIPO, ACAO_GUARDAR_TIPO, NT_PARAM_REST);
            AddEntry(NT_LISTAPARAM, T_RPAREN, T_EPSILON);

            // Regra 17: PARAM_REST -> , ident #ACAO_GUARDAR_IDENT_PARA_PARAM : TIPO #ACAO_GUARDAR_TIPO PARAM_REST | î
            AddEntry(NT_PARAM_REST, T_COMMA, T_COMMA, T_IDENT, ACAO_GUARDAR_IDENT_PARA_PARAM, T_COLON, NT_TIPO, ACAO_GUARDAR_TIPO, NT_PARAM_REST);
            AddEntry(NT_PARAM_REST, T_RPAREN, T_EPSILON);

            // Regra 18: BLOCO -> VAR_SEC_LOCAL begin COMANDOS end
            // Removidas as ações de escopo daqui, pois o escopo é gerenciado pela regra que chama BLOCO.
            AddEntry(NT_BLOCO, T_VAR, NT_VAR_SEC_LOCAL, T_BEGIN, NT_COMANDOS, T_END);
            AddEntry(NT_BLOCO, T_BEGIN, NT_VAR_SEC_LOCAL, T_BEGIN, NT_COMANDOS, T_END);

            // Regra 19: COMANDOS -> COMANDO CMD_REST | î
            AddEntry(NT_COMANDOS, T_PRINT, NT_COMANDO, NT_CMD_REST);
            AddEntry(NT_COMANDOS, T_IF, NT_COMANDO, NT_CMD_REST);
            AddEntry(NT_COMANDOS, T_IDENT, NT_COMANDO, NT_CMD_REST);
            AddEntry(NT_COMANDOS, T_FOR, NT_COMANDO, NT_CMD_REST);
            AddEntry(NT_COMANDOS, T_WHILE, NT_COMANDO, NT_CMD_REST);
            AddEntry(NT_COMANDOS, T_READ, NT_COMANDO, NT_CMD_REST);
            AddEntry(NT_COMANDOS, T_END, T_EPSILON);

            // Regra 20: CMD_REST -> ; COMANDO CMD_REST | î
            AddEntry(NT_CMD_REST, T_SEMICOLON, T_SEMICOLON, NT_COMANDO, NT_CMD_REST);
            AddEntry(NT_CMD_REST, T_END, T_EPSILON);

            // Regra 21: COMANDO -> print { ITEMSAIDA REPITEM }
            AddEntry(NT_COMANDO, T_PRINT, T_PRINT, T_LBRACE, NT_ITEMSAIDA, NT_REPITEM, T_RBRACE);

            // Regra 22: COMANDO -> if ( EXPRELACIONAL ) then #ACAO_INICIAR_ESCOPO BLOCO #ACAO_FINALIZAR_ESCOPO ELSEOPC
            AddEntry(NT_COMANDO, T_IF, T_IF, T_LPAREN, NT_EXPRELACIONAL, T_RPAREN, T_THEN, ACAO_INICIAR_ESCOPO, NT_BLOCO, ACAO_FINALIZAR_ESCOPO, NT_ELSEOPC);

            // Regra 23: COMANDO -> ident #ACAO_GUARDAR_IDENT_PARA_USO CMD_IDENT_REST
            AddEntry(NT_COMANDO, T_IDENT, T_IDENT, ACAO_GUARDAR_IDENT_PARA_USO, NT_CMD_IDENT_REST);

            // Regra 23b: COMANDO -> ε (quando vier 'end')
            AddEntry(NT_COMANDO, T_END, T_EPSILON);

            // Regra 24: CMD_IDENT_REST -> := EXPRESSAO #ACAO_VERIFICAR_ATRIB | CHAMADAPROC
            AddEntry(NT_CMD_IDENT_REST, T_ASSIGN, T_ASSIGN, NT_EXPRESSAO, ACAO_VERIFICAR_ATRIB);
            AddEntry(NT_CMD_IDENT_REST, T_LPAREN, NT_CHAMADAPROC);
            AddEntry(NT_CMD_IDENT_REST, T_SEMICOLON, NT_CHAMADAPROC);
            AddEntry(NT_CMD_IDENT_REST, T_END, NT_CHAMADAPROC);

            // Regra 25: CHAMADAPROC -> ( PARAMETROSCHAMADA ) | î
            AddEntry(NT_CHAMADAPROC, T_LPAREN, T_LPAREN, NT_PARAMETROSCHAMADA, T_RPAREN);
            AddEntry(NT_CHAMADAPROC, T_SEMICOLON, T_EPSILON);
            AddEntry(NT_CHAMADAPROC, T_END, T_EPSILON);
            // Regra 26: PARAMETROSCHAMADA -> EXPRESSAO PARAM_CHAMADA_REST | î
            AddEntry(NT_PARAMETROSCHAMADA, T_IDENT, NT_EXPRESSAO, NT_PARAM_CHAMADA_REST);
            AddEntry(NT_PARAMETROSCHAMADA, T_NINT, NT_EXPRESSAO, NT_PARAM_CHAMADA_REST);
            AddEntry(NT_PARAMETROSCHAMADA, T_NREAL, NT_EXPRESSAO, NT_PARAM_CHAMADA_REST);
            AddEntry(NT_PARAMETROSCHAMADA, T_LITERAL, NT_EXPRESSAO, NT_PARAM_CHAMADA_REST);
            AddEntry(NT_PARAMETROSCHAMADA, T_LPAREN, NT_EXPRESSAO, NT_PARAM_CHAMADA_REST);
            AddEntry(NT_PARAMETROSCHAMADA, T_RPAREN, T_EPSILON);

            // Regra 27: PARAM_CHAMADA_REST -> , EXPRESSAO PARAM_CHAMADA_REST | î
            AddEntry(NT_PARAM_CHAMADA_REST, T_COMMA, T_COMMA, NT_EXPRESSAO, NT_PARAM_CHAMADA_REST);
            AddEntry(NT_PARAM_CHAMADA_REST, T_RPAREN, T_EPSILON);

            // Regra 28: COMANDO -> for ident := EXPRESSAO to EXPRESSAO do #ACAO_INICIAR_ESCOPO BLOCO #ACAO_FINALIZAR_ESCOPO
            AddEntry(NT_COMANDO, T_FOR, T_FOR, T_IDENT, T_ASSIGN, NT_EXPRESSAO, T_TO, NT_EXPRESSAO, T_DO, ACAO_INICIAR_ESCOPO, NT_BLOCO, ACAO_FINALIZAR_ESCOPO);

            // Regra 29: COMANDO -> while ( EXPRELACIONAL ) do #ACAO_INICIAR_ESCOPO BLOCO #ACAO_FINALIZAR_ESCOPO
            AddEntry(NT_COMANDO, T_WHILE, T_WHILE, T_LPAREN, NT_EXPRELACIONAL, T_RPAREN, T_DO, ACAO_INICIAR_ESCOPO, NT_BLOCO, ACAO_FINALIZAR_ESCOPO);

            // Regra 30: COMANDO -> read ( ident )
            AddEntry(NT_COMANDO, T_READ, T_READ, T_LPAREN, T_IDENT, T_RPAREN);

            // Regra 31: ELSEOPC -> else #ACAO_INICIAR_ESCOPO BLOCO #ACAO_FINALIZAR_ESCOPO | î
            AddEntry(NT_ELSEOPC, T_ELSE, T_ELSE, ACAO_INICIAR_ESCOPO, NT_BLOCO, ACAO_FINALIZAR_ESCOPO);
            AddEntry(NT_ELSEOPC, T_SEMICOLON, T_EPSILON);
            AddEntry(NT_ELSEOPC, T_END, T_EPSILON);

            // Regra 32: ITEMSAIDA -> EXPRESSAO
            AddEntry(NT_ITEMSAIDA, T_IDENT, NT_EXPRESSAO);
            AddEntry(NT_ITEMSAIDA, T_NINT, NT_EXPRESSAO);
            AddEntry(NT_ITEMSAIDA, T_NREAL, NT_EXPRESSAO);
            AddEntry(NT_ITEMSAIDA, T_LITERAL, NT_EXPRESSAO);
            AddEntry(NT_ITEMSAIDA, T_LPAREN, NT_EXPRESSAO);

            // Regra 33: REPITEM -> , ITEMSAIDA REPITEM | î
            AddEntry(NT_REPITEM, T_COMMA, T_COMMA, NT_ITEMSAIDA, NT_REPITEM);
            AddEntry(NT_REPITEM, T_RBRACE, T_EPSILON);

            // Regra 34: EXPRELACIONAL -> EXPRESSAO OPREL EXPRESSAO
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
            AddEntry(NT_EXPRESSAO, T_IDENT, NT_TERMO, NT_EXPR_PRIME);
            AddEntry(NT_EXPRESSAO, T_NINT, NT_TERMO, NT_EXPR_PRIME);
            AddEntry(NT_EXPRESSAO, T_NREAL, NT_TERMO, NT_EXPR_PRIME);
            AddEntry(NT_EXPRESSAO, T_LITERAL, NT_TERMO, NT_EXPR_PRIME);
            AddEntry(NT_EXPRESSAO, T_LPAREN, NT_TERMO, NT_EXPR_PRIME);

            // Regra 37: EXPR' -> + TERMO #ACAO_CHECAR_OP_ARIT EXPR' | - TERMO #ACAO_CHECAR_OP_ARIT EXPR' | î
            AddEntry(NT_EXPR_PRIME, T_PLUS, T_PLUS, NT_TERMO, ACAO_CHECAR_OP_ARIT, NT_EXPR_PRIME);
            AddEntry(NT_EXPR_PRIME, T_MINUS, T_MINUS, NT_TERMO, ACAO_CHECAR_OP_ARIT, NT_EXPR_PRIME);
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
            AddEntry(NT_EXPR_PRIME, T_THEN, T_EPSILON);
            AddEntry(NT_EXPR_PRIME, T_END, T_EPSILON);

            // Regra 38: TERMO -> FATOR TER'
            AddEntry(NT_TERMO, T_IDENT, NT_FATOR, NT_TER_PRIME);
            AddEntry(NT_TERMO, T_NINT, NT_FATOR, NT_TER_PRIME);
            AddEntry(NT_TERMO, T_NREAL, NT_FATOR, NT_TER_PRIME);
            AddEntry(NT_TERMO, T_LITERAL, NT_FATOR, NT_TER_PRIME);
            AddEntry(NT_TERMO, T_LPAREN, NT_FATOR, NT_TER_PRIME);

            // Regra 39: TER' -> * FATOR #ACAO_CHECAR_OP_ARIT TER' | / FATOR #ACAO_CHECAR_OP_ARIT TER' | î
            AddEntry(NT_TER_PRIME, T_MULT, T_MULT, NT_FATOR, ACAO_CHECAR_OP_ARIT, NT_TER_PRIME);
            AddEntry(NT_TER_PRIME, T_DIV, T_DIV, NT_FATOR, ACAO_CHECAR_OP_ARIT, NT_TER_PRIME);
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
            AddEntry(NT_TER_PRIME, T_THEN, T_EPSILON);
            AddEntry(NT_TER_PRIME, T_END, T_EPSILON);

            // Regra 40: FATOR -> ident #ACAO_VERIFICAR_DECL | nint #ACAO_EMPILHAR_TIPO | nreal #ACAO_EMPILHAR_TIPO | literal #ACAO_EMPILHAR_TIPO | ( EXPRESSAO )
            AddEntry(NT_FATOR, T_IDENT, T_IDENT, ACAO_VERIFICAR_DECL);
            AddEntry(NT_FATOR, T_NINT, T_NINT, ACAO_EMPILHAR_TIPO);
            AddEntry(NT_FATOR, T_NREAL, T_NREAL, ACAO_EMPILHAR_TIPO);
            AddEntry(NT_FATOR, T_LITERAL, T_LITERAL, ACAO_EMPILHAR_TIPO);
            AddEntry(NT_FATOR, T_LPAREN, T_LPAREN, NT_EXPRESSAO, T_RPAREN);        }

        public bool Analisar(List<TokenInfo> tokensEntrada)
        {
            this.tokens = tokensEntrada;

            if (!this.tokens.Any() || this.tokens.Last().Lexema != T_EOF)
            {
                this.tokens.Add(new TokenInfo(0, T_EOF, tokens.LastOrDefault()?.Linha ?? 1));
            }

            tokenIndex = 0;
            errosSintaticos.Clear();
            analisadorSemantico.ErrosSemanticos.Clear();
            identificadoresParaDeclarar.Clear();
            parametrosParaDeclarar.Clear();

            pilha.Clear();
            pilha.Push(T_EOF); // Marcador de fundo da pilha
            pilha.Push(NT_PROGRAMA); // Símbolo inicial da gramática

            Console.WriteLine("\n--- Análise Sintática ---");
            analisadorSemantico.Tabela.ImprimirTabela("Inicial");

            do
            {
                string topo = pilha.Peek();
                TokenInfo tokenAtual = tokenIndex < tokens.Count ? tokens[tokenIndex] : new TokenInfo(0, "$", tokens.LastOrDefault()?.Linha ?? 1);
                string terminalAtual = MapearTokenParaTerminal(tokenAtual);

                Console.WriteLine($"Pilha: [ {string.Join(", ", pilha.Reverse())} ] | Token Atual: {terminalAtual} (	'{tokenAtual.Codigo}		' Linha: {tokenAtual.Linha})");

                if (topo.StartsWith("#ACAO"))
                {
                    pilha.Pop();
                    ExecutarAcaoSemantica(topo);
                    continue;
                }

                if (terminais.Contains(topo))
                {
                    if (topo == terminalAtual)
                    {
                        pilha.Pop();
                        tokenLidoAnteriormente = tokenAtual;
                        tokenIndex++;
                    }
                    else
                    {
                        RegistrarErroMatch(topo, tokenAtual);
                        tokenIndex++;
                        if (terminalAtual == T_EOF) break;
                    }
                }
                else if (naoTerminais.Contains(topo))
                {
                    if (tabelaParsing.ContainsKey((topo, terminalAtual)))
                    {
                        pilha.Pop();
                        string[] producao = tabelaParsing[(topo, terminalAtual)];
                        for (int i = producao.Length - 1; i >= 0; i--)
                        {
                            if (producao[i] != T_EPSILON)
                            {
                                pilha.Push(producao[i]);
                            }
                        }
                    }
                    else
                    {
                        RegistrarErroParsing(topo, tokenAtual);
                        tokenIndex++; 
                        Console.WriteLine($"[RECUPERAÇÃO] Descartando token 	'{terminalAtual}	' e continuando.");
                        if (terminalAtual == T_EOF) break;
                    }
                }
                else
                {
                    Console.WriteLine($"[ERRO INTERNO] Símbolo inesperado na pilha: {topo}");
                    errosSintaticos.Add($"Erro interno: Símbolo inesperado na pilha 	'{topo}	'");
                    return false;
                }

            } while (pilha.Peek() != T_EOF);

            bool sucessoSintatico = errosSintaticos.Count == 0;
            bool sucessoSemantico = analisadorSemantico.ErrosSemanticos.Count == 0;

            if (sucessoSintatico && sucessoSemantico)
            {
                Console.WriteLine("\nAnálise Sintática e Semântica concluída com sucesso!");
                return true;
            }
            else
            {
                Console.WriteLine("\nAnálise concluída com erros.");
                MostrarErros();
                return false;
            }
        }

        private void ExecutarAcaoSemantica(string acao)
        {
            switch (acao)
            {
                case ACAO_GUARDAR_IDENT_PARA_CONST:
                    identificadorConstante = tokenLidoAnteriormente;
                    break;

                case ACAO_INSERIR_CONST:
                    analisadorSemantico.Acao_InserirConstante(identificadorConstante, tokenLidoAnteriormente);
                    break;

                case ACAO_GUARDAR_IDENT_PARA_DECL:
                    identificadoresParaDeclarar.Add(tokenLidoAnteriormente);
                    break;

                case ACAO_INSERIR_VAR_LISTA:
                    foreach (var idToken in identificadoresParaDeclarar)
                    {
                        analisadorSemantico.Acao_InserirVariavel(idToken, tipoAtual);
                    }
                    identificadoresParaDeclarar.Clear();
                    break;

                case ACAO_GUARDAR_IDENT_PARA_PROC:
                    identificadorProcedure = tokenLidoAnteriormente;
                    break;

                case ACAO_INSERIR_PROC:
                    analisadorSemantico.Acao_InserirProcedure(identificadorProcedure);
                    break;

                case ACAO_GUARDAR_IDENT_PARA_PARAM:
                    parametrosParaDeclarar.Add(tokenLidoAnteriormente);
                    break;

                case ACAO_INSERIR_PARAM_LISTA:
                    foreach (var paramToken in parametrosParaDeclarar)
                    {
                        analisadorSemantico.Acao_InserirVariavel(paramToken, tipoAtual);
                    }
                    parametrosParaDeclarar.Clear();
                    break;

                case ACAO_GUARDAR_TIPO:
                    tipoAtual = tokenLidoAnteriormente.Lexema;
                    break;

                case ACAO_GUARDAR_IDENT_PARA_USO:
                    identificadoresParaDeclarar.Add(tokenLidoAnteriormente);
                    break;

                case ACAO_VERIFICAR_DECL:
                    analisadorSemantico.Acao_VerificarDeclaracao(tokenLidoAnteriormente);
                    break;

                case ACAO_VERIFICAR_ATRIB:
                    var variavelAtribuicao = identificadoresParaDeclarar.Last();
                    analisadorSemantico.Acao_VerificarAtribuicao(variavelAtribuicao);
                    identificadoresParaDeclarar.Remove(variavelAtribuicao);
                    break;

                case ACAO_EMPILHAR_TIPO:
                    analisadorSemantico.Acao_EmpilharTipo(tokenLidoAnteriormente);
                    break;

                case ACAO_CHECAR_OP_ARIT:
                    analisadorSemantico.Acao_ChecarTipoOperacaoAritmetica();
                    break;

                case ACAO_INICIAR_ESCOPO:
                    analisadorSemantico.Tabela.IniciarEscopo();
                    break;

                case ACAO_FINALIZAR_ESCOPO:
                    analisadorSemantico.Tabela.FinalizarEscopo();
                    break;
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
            if (terminais.Contains(token.Lexema)) return token.Lexema;

            Console.WriteLine($"[AVISO] Mapeamento: Token 	'{token.Lexema}	' (Código: {token.Codigo}) não é um terminal esperado diretamente. Usando lexema.");
            return token.Lexema;
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
            Console.WriteLine($"[ERRO] {msg}");
            if (!errosSintaticos.Contains(msg)) errosSintaticos.Add(msg);
        }

        public void MostrarErros()
        {
            if (errosSintaticos.Any())
            {
                Console.WriteLine("\n--- Erros Sintáticos Detectados ---");
                foreach (var erro in errosSintaticos.Distinct())
                {
                    Console.WriteLine(erro);
                }
            }

            if (analisadorSemantico.ErrosSemanticos.Any())
            {
                Console.WriteLine("\n--- Erros Semânticos Detectados ---");
                foreach (var erro in analisadorSemantico.ErrosSemanticos.Distinct())
                {
                    Console.WriteLine(erro);
                }
            }
        }
    }
}