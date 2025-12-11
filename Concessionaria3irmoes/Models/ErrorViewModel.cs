namespace Concessionaria3irmoes.Models;

/// <summary>
/// Modelo utilizado exclusivamente pela página de erro (View/Shared/Error.cshtml).
/// Esta classe NÃO é salva no banco de dados; serve apenas para transportar 
/// o ID da requisição que falhou, facilitando o diagnóstico do problema.
/// </summary>
public class ErrorViewModel
{
    // ID único da requisição HTTP (útil para verificar logs do servidor)
    public string? RequestId { get; set; }

    // Helper para decidir se deve ou não mostrar o ID na tela
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}