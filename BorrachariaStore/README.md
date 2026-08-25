# BorrachariaStore

Sistema de venda de produtos online (pneus e itens automotivos), desenvolvido em
C# (.NET) / ASP.NET Core com Razor Pages e persistência em MongoDB.

Disciplina: Planejamento de Projeto de Sistema Visual — Profa. Lian Hua Liu Iwersen

## Estrutura de pastas

```
BorrachariaStore/
├── BorrachariaStore.sln
├── docs/                              # PDFs de planejamento e cartões do Trello
├── src/
│   └── BorrachariaStore.Web/
│       ├── BorrachariaStore.Web.csproj
│       ├── Program.cs
│       ├── appsettings.json           # ConnectionString do MongoDB
│       ├── Models/
│       │   ├── Produto.cs
│       │   ├── ItemCarrinho.cs
│       │   ├── Pedido.cs
│       │   └── MongoDbSettings.cs
│       ├── Services/
│       │   ├── ProdutoService.cs      # Catálogo + CRUD admin (Aulas 3, 6, 7)
│       │   ├── CarrinhoService.cs     # Regras do carrinho (Aulas 4, 5)
│       │   └── PedidoService.cs       # Checkout (Aula 5)
│       ├── Data/MockData/
│       │   └── produtos.json          # Massa de dados inicial (Aula 2)
│       ├── Pages/
│       │   ├── Index.cshtml(.cs)      # Catálogo
│       │   ├── Carrinho.cshtml(.cs)
│       │   ├── Checkout.cshtml(.cs)
│       │   ├── Shared/_Layout.cshtml
│       │   └── Admin/
│       │       ├── Index.cshtml(.cs)      # Listagem (Aula 7)
│       │       ├── Cadastrar.cshtml(.cs)  # Insert (Aula 6)
│       │       └── Editar.cshtml(.cs)     # Update/Delete (Aula 7)
│       └── wwwroot/
│           ├── css/site.css
│           ├── js/site.js
│           └── img/
```

## Mapeamento com o cronograma (9 aulas)

| Aula | Data  | Onde mexer |
|------|-------|------------|
| 1 | 25/08 | Planejamento (este repositório) |
| 2 | 01/09 | Criar solução no Visual Studio a partir da pasta `src/`, ajustar `Data/MockData/produtos.json` |
| 3 | 15/09 | Configurar `MongoDbSettings` em `appsettings.json`, ativar `ProdutoService`, conectar `Pages/Index.cshtml` |
| 4 | 29/09 | `CarrinhoService` + `Pages/Carrinho.cshtml` |
| 5 | 06/10 | Cálculo de total + `Pages/Checkout.cshtml` + `PedidoService` |
| 6 | 20/10 | `Pages/Admin/Cadastrar.cshtml` (Insert) |
| 7 | 27/10 | `Pages/Admin/Index.cshtml` e `Editar.cshtml` (Update/Delete) |
| 8 | 03/11 | Testes de integração ponta a ponta |
| 9 | 10/11 | Refino visual, limpeza de código, build final |

## Como rodar (a partir da Aula 2)

```bash
cd src/BorrachariaStore.Web
dotnet restore
dotnet run
```

Pré-requisito: uma instância local ou remota do MongoDB, com a connection string
configurada em `appsettings.json` (seção `MongoDbSettings`).

## Fora do escopo

Calculadora de medidas de pneu, comparação lado a lado, notificações automáticas,
marketplace multi-vendedor, frete via API dos Correios e pagamentos reais.
