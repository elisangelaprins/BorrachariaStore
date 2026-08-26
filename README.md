# BorrachariaStore

Sistema visual de e-commerce automotivo (venda de pneus e serviços de borracharia), desenvolvido em C# (.NET 8) / ASP.NET Core com Razor Pages e persistência em banco de dados NoSQL (MongoDB).

**Disciplina:** Planejamento de Projeto de Sistema Visual — Profº. Lian Hua Liu Iwersen  

---

## 🎯 Proposta e Usuário-Alvo
* **Problema:** Dificuldade de motoristas encontrarem rapidamente especificações exatas de pneus (aro, largura, perfil) e realizarem pedidos com agilidade, além da necessidade de borracharias gerenciarem estoque e produtos de forma centralizada.
* **Público-Alvo:** Motoristas e proprietários de veículos automotivos (clientes) e gestores de borracharia/auto centers (administradores).

---

## ✨ Funcionalidades Principais (Escopo)
* **Catálogo Dinâmico:** Listagem visual de produtos com busca por nome e filtros por aro e preço.
* **Carrinho de Compras:** Adição/remoção de itens, controle de quantidade e cálculo automático de subtotal.
* **Checkout Simulado:** Formulário com validação de dados de entrega e seleção de método de pagamento (Cartão, Pix, Boleto).
* **Painel Administrativo (CRUD):** Área restrita para cadastro, listagem, edição e exclusão de pneus no MongoDB.

---

## 🛠️ Tecnologias Utilizadas
* **Linguagem:** C# (.NET 8)
* **Framework:** ASP.NET Core Razor Pages
* **Banco de Dados:** MongoDB (driver oficial `MongoDB.Driver`)
* **Front-end:** HTML5, CSS3, JavaScript, Bootstrap

---

## 📁 Estrutura de Pastas
```
BorrachariaStore/
├── .github/
│   └── CODEOWNERS                     # Regras de aprovação de código
├── .gitignore                         # Arquivos ignorados pelo Git (bin, obj, vs)
├── BorrachariaStore.sln               # Solução .NET
├── README.md                          # Documentação do projeto
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
