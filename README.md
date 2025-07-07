# Compilador Didático - Análise Léxica, Sintática e Semântica

Este projeto implementa as fases iniciais de um compilador para uma linguagem de programação simples, focando na análise léxica, na análise sintática descendente preditiva tabular (LL(1)) e na **análise semântica**.

![Tela Inicial](https://raw.githubusercontent.com/guijosegon/project-assets/master/Compilador/inicio.png )

## Funcionalidades

*   **Análise Léxica:** Identifica tokens na linguagem fonte com base em expressões regulares e uma tabela de símbolos. Reporta erros léxicos encontrados.
*   **Análise Sintática:** Utiliza um analisador descendente preditivo tabular (LL(1)) para verificar se a sequência de tokens segue a gramática da linguagem.
    *   **Tabela de Parsing:** A lógica de análise é guiada por uma tabela de parsing LL(1) pré-calculada (implementada no código `AnalisadorSintatico.cs`).
    *   **Visualização da Pilha:** Exibe o estado da pilha de análise a cada passo significativo (match de terminal ou aplicação de produção), facilitando a depuração e o entendimento do processo.
    *   **Tratamento de Erros:** Reporta erros sintáticos quando a análise não pode prosseguir conforme a tabela de parsing. Implementa uma recuperação de erro básica (modo pânico).
*   **Análise Semântica:** Verifica a correção lógica e de significado do código, garantindo a conformidade com as regras da linguagem.
    *   **Tabela de Símbolos:** Gerencia informações sobre identificadores (variáveis, constantes, procedimentos, parâmetros) e seus atributos (tipo, categoria, nível de escopo). A tabela é exibida a cada modificação.
    *   **Gerenciamento de Escopo:** Implementa o controle de visibilidade de identificadores em diferentes níveis de escopo (global, procedimentos, blocos de comandos como `if`, `while`, `for`).
    *   **Verificação de Tipos:** Valida a compatibilidade de tipos em atribuições e operações aritméticas, prevenindo operações inválidas (ex: somar texto com número).
    *   **Verificação de Declaração:** Garante que todos os identificadores utilizados foram previamente declarados e que não há declarações duplicadas no mesmo escopo.
    *   **Reporte de Erros Semânticos:** Identifica e reporta erros de significado com mensagens claras e indicação da linha.

## Componentes Principais

*   **`AnalisadorLexico.cs`**: Contém a lógica para dividir o código fonte em tokens (palavras reservadas, identificadores, números, símbolos, etc.) e identificar erros léxicos.
*   **`AnalisadorSintatico.cs`**: Implementa o analisador sintático LL(1) e orquestra as chamadas às ações semânticas.
    *   `InicializarTabelaParsing()`: Define a tabela de parsing que guia a análise, agora incluindo as ações semânticas.
    *   `Analisar()`: Executa o algoritmo de análise sintática e semântica usando a pilha e a tabela.
    *   `ExecutarAcaoSemantica()`: Método responsável por despachar as chamadas para as ações semânticas específicas.
    *   `RegistrarErro...()`: Funções para reportar erros sintáticos.
*   **`AnalisadorSemantico.cs`**: Contém a lógica das ações semânticas, como verificação de tipos, declarações e manipulação da tabela de símbolos.
*   **`TabelaDeSimbolos.cs`**: Gerencia a inserção, busca e remoção de símbolos, além do controle de escopo.
*   **`Simbolo.cs`**: Define a estrutura de dados para cada entrada na tabela de símbolos.
*   **`Program.cs`**: Gerencia a execução, permite a seleção do arquivo de entrada, chama os analisadores léxico, sintático e semântico, e exibe os resultados e erros.

## Como Compilar e Executar

1.  **Pré-requisitos:** Certifique-se de ter o SDK do .NET 8.0 (ou compatível) instalado.
2.  **Navegar até o Diretório:** Abra um terminal ou prompt de comando e navegue até o diretório `CompiladorParaConsole`.
3.  **Executar:** Execute o comando:
    ```bash
    dotnet run
    ```
4.  **Selecionar Exemplo:** O programa solicitará que você digite o número correspondente ao arquivo de código que deseja analisar:
    *   `1`: `exemplo1.txt`
    *   `2`: `exemplo2.txt`
    *   `3`: `exemplo3.txt`
5.  **Analisar Saída:** Observe a saída no console, que incluirá:
    *   A lista de tokens identificados.
    *   Erros léxicos (se aplicável).
    *   **O estado da Tabela de Símbolos a cada modificação.**
    *   Os passos da análise sintática com a visualização da pilha.
    *   Erros sintáticos (se aplicável).
    *   **Erros semânticos (se aplicável).**

## Arquivos de Exemplo

*   **`exemplo1.txt`**: Demonstra declarações de constantes e variáveis de diferentes tipos, comandos `print` e `read`, e comentários. **(Análise semântica: verifica declarações e tipos básicos)**
*   **`exemplo2.txt`**: Demonstra a definição e chamada de `procedure` (com e sem parâmetros), o uso de `if`/`then`/`else` e o loop `while`. **(Análise semântica: verifica escopo de procedures, parâmetros e blocos de comandos)**
*   **`exemplo3.txt`**: Demonstra o loop `for`, expressões aritméticas e relacionais dentro de comandos. **(Análise semântica: verifica expressões complexas e escopo de `for`)**

## Gramática e Tabela de Parsing

*   **`gramatica_ll1.txt`**: Contém a gramática formal utilizada, adaptada para ser LL(1).
*   **`first_follow.txt`**: Apresenta uma estimativa dos conjuntos FIRST e FOLLOW calculados para a gramática.
*   A tabela de parsing LL(1) está implementada diretamente no método `InicializarTabelaParsing()` dentro de `AnalisadorSintatico.cs`, **agora incorporando as ações semânticas**.
