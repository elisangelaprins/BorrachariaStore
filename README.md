# BorrachariaStore

Sistema visual de e-commerce automotivo (venda de pneus e serviços de borracharia), desenvolvido em C# (.NET 8) / ASP.NET Core com Razor Pages e persistência em banco de dados NoSQL (MongoDB).

**Disciplina:** Planejamento de Projeto de Sistema Visual — Profº. Lian Hua Liu Iwersen  

---

## 🎯 Proposta e Usuário-Alvo
* **Problema:** Dificuldade de motoristas encontrarem rapidamente especificações exatas de pneus (aro, largura, perfil) e realizarem pedidos com agilidade, além da necessidade de borracharias gerenciarem estoque e produtos de forma centralizada.
* **Público-Alvo:** Motoristas e proprietários de veículos automotivos (clientes) e gestores de borracharia/auto centers (administradores).

---

## ✨ Funcionalidades Principais (Escopo)
* **Catálogo & Vitrine Dinâmica:** Listagem de produtos com busca em tempo real, filtros por aro, categoria, ordenação e página dedicada (`/Catalogo`).
* **Regras de Promoção & Ofertas:** Produtos com preço promocional, porcentagem de desconto (`-X% OFF`), preço original riscado e filtro de ofertas da semana.
* **Drawer de Detalhes Rápidos:** Modal lateral deslizante com especificações técnicas, medidas e galeria de fotos sem sair da página.
* **Carrinho de Compras & Cupons:** Controle de itens/quantidades, adição assíncrona via API (sem reload da página) com contador em tempo real no cabeçalho, cálculo de frete e aplicação de cupons de desconto.
* **Checkout Simulado:** Formulário com validação de dados de entrega e seleção de método de pagamento (Cartão, Pix, Boleto).
* **Consulta de CEP (ViaCEP):** Preenchimento automático de endereço de entrega no checkout com resiliência e tratamento de timeout.
* **Autenticação (Login):** Proteção de acesso às áreas administrativas.
* **Tratamento Amigável de Erros:** Página customizada (`/Error`) para falhas inesperadas com atalhos de navegação.
* **Painel Administrativo Completo:**
  * **Estoque (CRUD):** Cadastro, listagem em tempo real, edição e exclusão de pneus no MongoDB.
  * **Upload de Imagens:** Suporte a upload de múltiplos arquivos de fotos locais (`IFormFile`) com armazenamento no servidor.
  * **Cupons:** Criação e ativação/desativação de cupons promocionais com regras de valor mínimo.

---

## 🛠️ Tecnologias Utilizadas
* **Linguagem:** C# (.NET 8)
* **Framework:** ASP.NET Core Razor Pages
* **Banco de Dados:** MongoDB (driver oficial `MongoDB.Driver`)
* **Front-end:** HTML5, CSS3 (Customizado / Vanilla), JavaScript

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
│       │   ├── Produto.cs             # Modelo com suporte a preços promocionais
│       │   ├── Cupom.cs               # Modelo de cupom de desconto
│       │   ├── ItemCarrinho.cs
│       │   ├── Pedido.cs
│       │   ├── EnderecoCepDto.cs      # Modelo de retorno da API de CEP
│       │   └── MongoDbSettings.cs
│       ├── Services/
│       │   ├── ProdutoService.cs      # Catálogo e operações de CRUD no MongoDB
│       │   ├── CarrinhoService.cs     # Gerenciamento de itens, totais e cupons
│       │   ├── PedidoService.cs       # Checkout e registro de pedidos
│       │   ├── CupomService.cs        # Validação e persistência de cupons
│       │   └── ViaCepService.cs        # Integração com a API do ViaCEP
│       ├── Data/MockData/
│       │   └── produtos.json          # Massa de dados inicial (Seed com promoções)
│       ├── Pages/
│       │   ├── Index.cshtml(.cs)      # Página inicial (Home) com destaques
│       │   ├── Catalogo.cshtml(.cs)   # Catálogo completo com filtros avançados
│       │   ├── Carrinho.cshtml(.cs)   # Gestão de itens e aplicação de cupom
│       │   ├── Checkout.cshtml(.cs)   # Formulário de entrega e pagamento
│       │   ├── Login.cshtml(.cs)      # Acesso restrito ao Painel Admin
│       │   ├── Error.cshtml(.cs)      # Página customizada de tratamento de erro
│       │   ├── Shared/
│       │   │   ├── _Layout.cshtml     # Layout mestre
│       │   │   └── _DetalhesProduto.cshtml # Modal lateral de detalhes do produto
│       │   └── Admin/
│       │       ├── Index.cshtml(.cs)      # Painel de estoque (listagem e exclusão)
│       │       ├── Cadastrar.cshtml(.cs)  # Cadastro de novo produto
│       │       ├── Editar.cshtml(.cs)     # Edição de produto existente
│       │       └── Cupons.cshtml(.cs)     # Gestão e criação de cupons
│       └── wwwroot/
│           ├── css/
│           │   ├── site.css           # Estilos principais da loja
│           │   ├── catalogo.css       # Estilos da página de catálogo e filtros
│           │   ├── detalhes-produto.css # Estilos do drawer de detalhes rápidos
│           │   ├── admin.css          # Estilos do painel administrativo
│           │   ├── carrinho.css       # Estilos do carrinho
│           │   ├── checkout.css       # Estilos do checkout
│           │   └── login.css          # Estilos do login
│           ├── js/
│           │   └── site.js            # Scripts, modais e interações
│           └── images/                # Imagens (logos, marcas, produtos e hero)
```

---

## 🚀 Como Executar o Projeto

```bash
cd src/BorrachariaStore.Web
dotnet restore
dotnet run
```

Pré-requisito: uma instância local ou remota do MongoDB, com a connection string
configurada em `appsettings.json` (seção `MongoDbSettings`).

---

## 🔐 Acesso ao Painel Administrativo

Para acessar a área restrita de gestão (produtos, estoque e cupons):

1. No navegador com o projeto rodando, acesse a rota:
   ```
   https://localhost:<porta>/Login
   ```
   *(ou acesse diretamente pelo endereço `/Admin`)*

2. Utilize as credenciais de demonstração configuradas na aplicação:
   * **E-mail:** `admin@borracharia.com`
   * **Senha:** `admin123`

3. **Recursos disponíveis no painel:**
   * **Gerenciador de Estoque:** `/Admin/Index` (listagem em tempo real e remoção de itens)
   * **Novo Produto:** `/Admin/Cadastrar` (cadastro com aro, medidas, preço normal e promocional)
   * **Edição de Produto:** `/Admin/Editar?id=<id>` (alteração de preços, fotos e especificações)
   * **Cupons de Desconto:** `/Admin/Cupons` (criação e ativação/desativação de cupons promocionais)

## Fora do escopo

Calculadora de medidas de pneu, comparação lado a lado, notificações automáticas,
marketplace multi-vendedor, frete via API dos Correios e pagamentos reais.
