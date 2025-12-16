using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Concessionaria3irmoes.Models
{
    public class VeiculoFoto
    {
        public int Id { get; set; }
        public string CaminhoArquivo { get; set; } = string.Empty;
        public int VeiculoId { get; set; }
        public VeiculoModel? Veiculo { get; set; }
    }
}