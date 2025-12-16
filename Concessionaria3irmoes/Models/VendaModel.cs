using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Concessionaria3irmoes.Models
{
    public class VendaModel
    {
        public int Id { get; set; }
        public DateTime DataVenda { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal Desconto { get; set; }
        public decimal ValorFinal { get; set; }

    // Relacionamentos
    public int ClienteId { get; set; }
    public virtual ClienteModel? Cliente { get; set; }

    public int VeiculoId { get; set; }
    public virtual VeiculoModel? Veiculo { get; set; }
    }
}