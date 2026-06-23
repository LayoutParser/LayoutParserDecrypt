# Dex (Desenvolvedor) — Memória do Agente

Log append-only de aprendizados de engenharia, ancorado em caminhos reais.

## Key Patterns

### Build é msbuild clássico, não dotnet CLI
- Target `v4.8.1`, `OutputType=Exe`. Build: `msbuild .\LayoutParserDecrypt.sln /m /p:Configuration=Release /p:Platform="Any CPU"` (precedido de `nuget restore`).
- Não existe `dotnet test` nem projeto de teste. Validação = build + roundtrip manual (`/decrypt-roundtrip`).

### Lib é vendorizada como fontes locais (`LayoutParserDecrypt.csproj`)
- O `.csproj` faz `<Compile Include="LayoutParserLib\CryptographySysMiddle.cs" />` e `RollingFileLogger.cs`. Não há `<ProjectReference>` ao repo `LayoutParserLib`. Motivo (comentário no csproj): CI no GitHub Actions precisa compilar sem o sibling.
- Existe um `LayoutParserLib.dll` commitado na raiz — legado/vestigial; o build não depende dele.

### Dois loggers distintos
- `RollingFileLogger.cs` (namespace `LayoutParserDecrypt`) → `layoutparserdecrypt.log`, assinatura `Log(logDir, baseFileName, corr, level, msg, ex?)`.
- `LayoutParserLib/RollingFileLogger.cs` (namespace `LayoutParserLib`) → `layoutparserlib.log`, usa `Configure(logDir, corr)` + `Log(level, msg, ex?)`.
- Ambos: rotação em ~2MB (`2049*1024`), mantêm 10 arquivos, `catch {}` engole falhas (logging nunca lança).

## Gotchas
- **CRITICAL:** o `Substring(3)` (strip de prefixo) está em [`Program.cs`](../../../Program.cs), NÃO em `CryptographySysMiddle.Decrypt`. Mudar isso afeta a API.
- **CRITICAL:** contrato de CLI `[input, output, correlationId?, logDir?]` + exit codes `0/1` é consumido por `DecryptionService.cs` da `LayoutParserApi`. Não quebrar.
- Chave/IV hardcoded em `CryptographySysMiddle.cs`. Não duplicar segredos.
- Ao mudar a cripto, sincronizar com o repo canônico `..\LayoutParserLib` (duas cópias divergem fácil).
