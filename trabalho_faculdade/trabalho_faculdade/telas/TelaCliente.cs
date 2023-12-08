using Newtonsoft.Json;
using trabalhopoo.db;
using trabalhopoo.json;
using trabalhopoo.models;
using trabalhopoo.util;

namespace trabalhopoo.telas;

public static class TelaCliente
{
  // Listagem de clientes
  public static void listClientScreen()
  {
    List<Cliente> clientes = clienteUtil.listaClientes();

    if (clientes.Count == 0)
    {
      Console.WriteLine("Nenhum cliente encontrado.");
      ControleSemClientes();
      return;
    }
      
    Console.WriteLine("Lista de Clientes");
    Console.WriteLine("-------------------------------------------------");

    Console.WriteLine("Id\t\tNome\t\tCpf");
    Console.WriteLine("-------------------------------------------------");
    
    foreach (Cliente cliente in clientes)
    {
      Console.WriteLine($"{cliente.Id}\t\t{cliente.Nome}\t\t{cliente.Cpf}");
    }

    ControleClientes();
  }

  public static void ControleSemClientes()
  {
    Console.WriteLine("-------------------------------------------------");
      
    Console.WriteLine("Escolha a opção:");
    Console.WriteLine("1. Cadastrar novo cliente");
    Console.WriteLine("2. Fechar o programa");
    Console.Write("Escolha uma opção (1 ou 2): ");
      
    string op = Read.ReadOptions(new[] { "1", "2" });
      
    switch (op)
    {
      case "1":
        newClientScreen();
        break;
      case "2":
        return;
      default:
        Console.WriteLine("Opção inválida.");
        break;
    }
  }
  
  public static void ControleClientes()
  {
    Console.WriteLine("-------------------------------------------------");
      
    Console.WriteLine("Escolha a opção:");
    Console.WriteLine("1. Cadastrar novo cliente");
    Console.WriteLine("2. Listar dados dos imóveis de um cliente");
    Console.Write("Escolha uma opção (1 ou 2): ");
      
    string op = Read.ReadOptions(new[] { "1", "2" });
      
    switch (op)
    {
      case "1":
        newClientScreen();
        break;
      case "2":
        TelaImovel.listInformacoesScreen(null);
        break;
      default:
        Console.WriteLine("Opção inválida.");
        break;
    }
  }

  // Opções cadastro de cliente
  public static void newClientScreen()
  {
    Console.Clear();
    Console.WriteLine("---------------------------------------");
    Console.WriteLine("Cadastrar cliente");
    Console.WriteLine("---------------------------------------");
    Console.WriteLine("Escolha a opção:");
    Console.WriteLine("1. Inserir cliente manualmente");
    Console.WriteLine("2. Inserir cliente(s) via arquivo JSON");
    Console.Write("Escolha uma opção (1 ou 2): ");
            
    string op = Read.ReadOptions(new string[] { "1", "2" });
            
    switch (op)
    {
      case "1":
        CadastrarClienteManualmente();
        break;
      case "2":
        CadastrarClientePorJson();
        break;
      default:
        Console.WriteLine("Opção inválida.");
        break;
    }
  }
  
  // Cadastrar cliente manual
  private static void CadastrarClienteManualmente()
  {
    Console.Write("Nome do Cliente: ");
    string nome = Read.ReadStringMax(150);
    Console.Write("Cpf do Cliente: (Informe o CPF sem pontuação)");
    string cpf = Read.ReadCpf();

    if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(cpf))
    {
      Console.WriteLine("Operação cancelada.");
      Console.WriteLine("\n");
      TelaCliente.listClientScreen();
      return;
    }

    try
    {
      clienteUtil.AdicionarCliente(nome, cpf);        
      Console.WriteLine("\n");
      TelaCliente.listClientScreen();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Ocorreu um erro: {ex.Message}");
      TelaCliente.listClientScreen();
    }
  }
  
  // Cadas cliente via JSON
  private static void CadastrarClientePorJson()
  {
    string caminhoJson = "";
    bool pathValido = false;

    while (!pathValido)
    {
      Console.Write("Digite o caminho completo do arquivo JSON: ");
      caminhoJson = Read.ReadString();

      if (string.IsNullOrEmpty(caminhoJson) || !File.Exists(caminhoJson))
      {
        Console.WriteLine("Arquivo não encontrado ou caminho inválido.");
        continue;
      }

      pathValido = true;
    }

    try
    {
      string json = File.ReadAllText(caminhoJson);
      List<ClienteJson>? clientesJson = JsonConvert.DeserializeObject<List<ClienteJson>>(json);

      if (clientesJson != null)
      {
        foreach (var clienteJson in clientesJson)
          clienteUtil.AdicionarCliente(clienteJson.Nome, clienteJson.Cpf);
        
        Console.WriteLine("\n");
        TelaCliente.listClientScreen();
        return;
      }
      
      Console.WriteLine("Nenhum valor encontrado para ser inserido.");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Ocorreu um erro: {ex.Message}");
      TelaCliente.listClientScreen();
    }
  }
}