# Compilador Didático - Análise Léxica e Sintática

Este projeto implementa as fases iniciais de um compilador para uma linguagem de programação simples, focando na análise léxica e na análise sintática descendente preditiva tabular (LL(1)).

## Funcionalidades

*   **Análise Léxica:** Identifica tokens na linguagem fonte com base em expressões regulares e uma tabela de símbolos. Reporta erros léxicos encontrados.
*   **Análise Sintática:** Utiliza um analisador descendente preditivo tabular (LL(1)) para verificar se a sequência de tokens segue a gramática da linguagem.
    *   **Tabela de Parsing:** A lógica de análise é guiada por uma tabela de parsing LL(1) pré-calculada (implementada no código `AnalisadorSintatico.cs`).
    *   **Visualização da Pilha:** Exibe o estado da pilha de análise a cada passo significativo (match de terminal ou aplicação de produção), facilitando a depuração e o entendimento do processo.
    *   **Tratamento de Erros:** Reporta erros sintáticos quando a análise não pode prosseguir conforme a tabela de parsing. Implementa uma recuperação de erro básica (modo pânico).
*   **Seleção de Exemplos:** Permite ao usuário escolher, via console, qual arquivo de código fonte analisar.

## Estrutura do Projeto

```
CompiladorParaConsole/
├── AnalisadorLexico.cs       # Implementação do analisador léxico
├── AnalisadorSintatico.cs    # Implementação do analisador sintático (LL(1) tabular)
├── Program.cs                # Ponto de entrada, orquestra as análises e interação com usuário
├── CompiladorParaConsole.csproj # Arquivo de projeto .NET
├── exemplo1.txt              # Exemplo: Declarações, I/O, Comentários
├── exemplo2.txt              # Exemplo: Procedures, If/Else, While
├── exemplo3.txt              # Exemplo: For, Expressões
├── gramatica_ll1.txt         # Gramática LL(1) derivada do manual (usada para a tabela)
├── first_follow.txt          # Conjuntos FIRST/FOLLOW (estimativa) derivados da gramática
└── README.md                 # Este arquivo
```

## Componentes Principais

*   **`AnalisadorLexico.cs`**: Contém a lógica para dividir o código fonte em tokens (palavras reservadas, identificadores, números, símbolos, etc.) e identificar erros léxicos.
*   **`AnalisadorSintatico.cs`**: Implementa o analisador sintático LL(1).
    *   `InicializarTabelaParsing()`: Define a tabela de parsing que guia a análise.
    *   `Analisar()`: Executa o algoritmo de análise sintática usando a pilha e a tabela.
    *   `MostrarPilha()`: Exibe o conteúdo da pilha durante a análise.
    *   `RegistrarErro...()`: Funções para reportar erros sintáticos.
*   **`Program.cs`**: Gerencia a execução, permite a seleção do arquivo de entrada, chama os analisadores léxico e sintático, e exibe os resultados e erros.

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
    *   Os passos da análise sintática com a visualização da pilha.
    *   Erros sintáticos (se aplicável).

## Arquivos de Exemplo

*   **`exemplo1.txt`**: Demonstra declarações de constantes e variáveis de diferentes tipos, comandos `print` e `read`, e comentários.
*   **`exemplo2.txt`**: Demonstra a definição e chamada de `procedure` (com e sem parâmetros), o uso de `if`/`then`/`else` e o loop `while`.
*   **`exemplo3.txt`**: Demonstra o loop `for`, expressões aritméticas e relacionais dentro de comandos.

## Gramática e Tabela de Parsing

*   **`gramatica_ll1.txt`**: Contém a gramática formal utilizada, adaptada para ser LL(1).
*   **`first_follow.txt`**: Apresenta uma estimativa dos conjuntos FIRST e FOLLOW calculados para a gramática.
*   A tabela de parsing LL(1) está implementada diretamente no método `InicializarTabelaParsing()` dentro de `AnalisadorSintatico.cs`.

## Limitações e Próximos Passos

*   A gramática e a tabela de parsing foram derivadas do manual e podem precisar de refinamentos.
*   A recuperação de erros sintáticos é básica.
*   O projeto foca nas fases de análise léxica e sintática. As próximas fases (análise semântica, geração de código intermediário, otimização, geração de código final) não estão implementadas.

