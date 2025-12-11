using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Concessionaria3irmoes.Models
{
    public class ClienteModel
    {
        public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Endereco { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    }
}