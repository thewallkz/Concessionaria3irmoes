using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Concessionaria3irmoes.Models
{
    public class VeiculoModel
    {
        public int Id { get; set; }
    public string Modelo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public string Motor { get; set; } = string.Empty;
    public string Potencia { get; set; } = string.Empty;
    public string Quilometragem { get; set; } = string.Empty;
    public string Ano { get; set; } = string.Empty;
    public bool Vendido { get; set; }
    }

}