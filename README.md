# Concessionária 3 Irmoes

## Sobre o Projeto
Esta é uma aplicação web desenvolvida utilizando o padrão **MVC** em **ASP.NET Core .NET 8**. O projeto foi realizado no âmbito da disciplina de Desenvolvimento Web, simulando o sistema de gestão de uma concessionária de veículos.

O sistema permite a gestão completa de veículos, clientes e vendas, com controlo de acesso para garantir a segurança das operações administrativas.

## Funcionalidades Principais
- **Gestão de Veículos:** Registo, edição, visualização e remoção de veículos.
- **Vendas:** Registo de vendas vinculando clientes e veículos, com atualização automática do estado do veículo para "Vendido".
- **Autenticação e Autorização:** Sistema de login utilizando **ASP.NET Identity**. Apenas utilizadores com perfil de 'Admin' podem gerir o stock.
- **Layout Responsivo:** Interface construída com **Bootstrap** para funcionar em computadores e dispositivos móveis.

## Tecnologias Utilizadas
- **Linguagem:** C#
- **Framework:** ASP.NET Core MVC
- **Base de Dados:** SQLite (via Entity Framework Core)
- **Frontend:** Razor Views, Bootstrap

## Como Executar o Projeto

### Pré-requisitos
- .NET 8 SDK instalado.
- Visual Studio 2022 ou Visual Studio Code.

### Passo a Passo
1. **Clonar ou descarregar o repositório** para a máquina local.
2. Abrir o terminal na pasta raiz do projeto `Concessionaria3irmoes`.
3. **Restaurar os pacotes** necessários:
   ```bash
   dotnet watch
## Organizacao
### Equipe
- Wilyan - Views
- Felipe - Controllers
- Bruno - Models
