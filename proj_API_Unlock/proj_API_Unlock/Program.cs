using proj_API_Unlock;
using System;
using System.Net.Http;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            RunAsync().GetAwaiter().GetResult(); // Abrir
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao executar:");
            Console.WriteLine(ex.Message);
        }

        Console.WriteLine("Pressione qualquer tecla para sair...");
        Console.ReadKey();
    }

    private static async Task RunAsync()
    {
        
        string clientId = "05eb88a940484bd08e8f2f263e56eb2b";
        string accessToken = "1468737f4460a3a39bc2ca5186ca1097";
        int lockId = 17097086; 

        using (var httpClient = new HttpClient())
        {
            var ttlock = new TtlockClient(httpClient, clientId, accessToken);

            Console.WriteLine("Enviando comando de UNLOCK para a fechadura...");

            bool ok = await ttlock.UnlockAsync(lockId);

            if (ok)
            {
                Console.WriteLine("Fechadura destravada com sucesso (errcode = 0).");
            }
        }
    }
}
