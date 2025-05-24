
# Projeto Compilador

Este repositório contém um projeto completo de um analisador léxico, parte integrante da construção de um compilador (Incompleto), desenvolvido em C# com .NET 8. Com interface gráfica e suporte a execução em console.

## 🧱 Estrutura do Projeto

- **CompiladorComInterface** – Versão com Windows Forms para visualização gráfica dos tokens e erros.
- **CompiladorParaConsole** – Versão em terminal para testes rápidos e execução via linha de comando.
- **AnalisadorLexico.cs** – Módulo reutilizável de análise léxica com regex, tokens e tratamento de erros.
- **codigo.txt** – Arquivo de entrada para testes, contendo exemplos com e sem erros.

## 📘 Funcionalidades

- Identificação de tokens conforme gramática definida.
- Reconhecimento de:
  - Palavras-chave
  - Identificadores
  - Números inteiros e reais
  - Strings
  - Operadores e símbolos
- Detecção de erros léxicos como:
  - Strings não finalizadas
  - Tokens inválidos (ex: `123abc`, `:=:`)
- Visualização em tabela (GUI) e saída de console.
- Interface limpa e intuitiva para uso acadêmico.

## 🚀 Como executar

### Console
```bash
1. Abra a solução no Visual Studio
2. Clique com o botão direito em `CompiladorParaConsole` > Set as Startup Project
3. Pressione F5
```

### Interface Gráfica
```bash
1. Clique com o botão direito em `CompiladorComInterface` > Set as Startup Project
2. Pressione F5
```

## 📂 Organização

```plaintext
ProjetoCompilador/
├── src/
│   ├── CompiladorComInterface/
│   └── CompiladorParaConsole/
├── README.md
└── ManualCompilador.pdf (documentação acadêmica)
```

## 👨‍💻 Tecnologias

- C# (.NET 8)
- Windows Forms
- Visual Studio 2022+
