using System;

// Script helper para gerar hash de passwords
// Executar com: dotnet run --project GenerateHash.cs

class GenerateHash
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Gerador de Hash BCrypt ===\n");
        
        if (args.Length > 0)
        {
            var password = args[0];
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"Hash: {hash}");
        }
        else
        {
            // Gerar hashes para passwords padrão
            var passwords = new[] { "admin123", "teste123", "clinica123" };
            
            foreach (var password in passwords)
            {
                var hash = BCrypt.Net.BCrypt.HashPassword(password);
                Console.WriteLine($"Password: {password}");
                Console.WriteLine($"Hash: {hash}\n");
            }
            
            Console.WriteLine("\nPara gerar hash de outra password, execute:");
            Console.WriteLine("dotnet run <password>");
        }
    }
}
