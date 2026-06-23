# decrypt-roundtrip

Compila em Release e executa uma descriptografia de teste para validar uma mudança comportamental.

## O que este comando faz
1. `nuget restore` + build Release via msbuild.
2. Executa o `.exe` contra um arquivo de entrada criptografado conhecido e grava a saída.
3. Verifica o exit code (`0`) e inspeciona a saída (deve ser XML legível: `/LayoutVO` ou `/MapperVO`).

## Execução
```powershell
nuget restore .\LayoutParserDecrypt.sln
msbuild .\LayoutParserDecrypt.sln /m /p:Configuration=Release /p:Platform="Any CPU"

# substitua pelo caminho de um arquivo real criptografado da Sysmiddle
$in  = ".\samples\entrada.b64"
$out = ".\samples\saida.xml"
.\bin\Release\LayoutParserDecrypt.exe $in $out "roundtrip-test" ".\logs"
Write-Host "ExitCode: $LASTEXITCODE"
Get-Content $out -TotalCount 5
```

## Regras
- Se não houver amostra real disponível, peça uma ao usuário — **não** fabrique um Base64 de teste esperando que descriptografe (a chave/IV são fixas e específicas da Sysmiddle).
- Exit code ≠ 0 ou saída vazia/ilegível = falha; investigue os logs em `.\logs\layoutparserdecrypt.log` e `layoutparserlib.log`.
- Não commitar amostras com dados sensíveis.
