# sync-vendored-lib

Compara as cópias vendorizadas da cripto/logger com o repositório canônico `LayoutParserLib` e ajuda a sincronizá-las.

## O que este comando faz
1. Localiza o repo canônico: `..\LayoutParserLib` (relativo à raiz deste repo).
2. Faz diff de:
   - `LayoutParserLib/CryptographySysMiddle.cs`  ⇄  `..\LayoutParserLib\CryptographySysMiddle.cs`
   - `LayoutParserLib/RollingFileLogger.cs`  ⇄  `..\LayoutParserLib\RollingFileLogger.cs`
3. Reporta as diferenças e pergunta a direção da sincronização (canônico → cópia, normalmente).

## Execução
```bash
# diffs (rode da raiz deste repo)
git --no-pager diff --no-index ".\LayoutParserLib\CryptographySysMiddle.cs" "..\LayoutParserLib\CryptographySysMiddle.cs"
git --no-pager diff --no-index ".\LayoutParserLib\RollingFileLogger.cs"     "..\LayoutParserLib\RollingFileLogger.cs"
```

## Regras
- Fonte da verdade = repo `LayoutParserLib`. Em caso de divergência, o canônico vence, salvo decisão explícita do usuário.
- Se o repo canônico não existir localmente, avise e não invente conteúdo.
- Após sincronizar, rode `/decrypt-roundtrip` para garantir que ainda compila e descriptografa.
