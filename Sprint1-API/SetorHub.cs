using Microsoft.AspNetCore.SignalR;

namespace Sprint1_API;

public class SetorHub:Hub
{
 
    // Método chamado quando o cliente se conecta e escolhe um grupo (pátio específico)
    public async Task EntrarNoPatio(string patioId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"patio-{patioId}");
    }
    
    public async Task SairDoPatio(string patioId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"patio-{patioId}");
    }
    
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"Cliente conectado: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Cliente desconectado: {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }
    
}