using Newtonsoft.Json;
using trabalhopoo.db;
using trabalhopoo.json;
using trabalhopoo.models;
using trabalhopoo.util;

namespace trabalhopoo.telas;

public class TelaConta
{
  public static void NewContaControle(string matricula)
  {
    Console.WriteLine("\n");
    Console.WriteLine("Escolha a opção:");
    Console.WriteLine("1. Inserir conta manualmente");
    Console.WriteLine("2. Inserir conta(s) via arquivo JSON");
    Console.WriteLine("3. Voltar ao inicio");
    Console.Write("Escolha uma opção (1 , 2 ou 3): ");

    string op = Read.ReadOptions(new[] { "1", "2", "3" });
        
    switch (op)
    {
      case "1":
        CadastrarContaManualmente(matricula);
        break;
      case "2":
        CadastrarContaViaJson(matricula);
        break;
      case "3":
        Console.Clear();
        TelaCliente.listClientScreen();
        break;
      default:
        Console.WriteLine("Opção inválida.");
        break;
    }
  }
  
  // Cadastrar conta manualmente
  private static void CadastrarContaManualmente(string matricula)
  {
      Imovel? imovel = imovelUtil.GetImovelByMatricula(matricula);
      
      if(imovel == null)
      {
        Console.WriteLine("Houve um erro ao cadastrar! Imovél não encontrado.");
        return;
      }

      Conta c = new Conta();
      c.Imovel = imovel;
      
      Console.WriteLine("Digite o tipo da conta:");
      Console.WriteLine("1. Água");
      Console.WriteLine("2. Luz");
      
      string op = Read.ReadOptions(new[] { "1", "2" });
      
      switch (op)
      {
          case "1":
              c.TipoConta = TipoConta.Agua;
              break;
          case "2":
              c.TipoConta = TipoConta.Luz;
              break;
          default:
              Console.WriteLine("Não foi possível definir um valor");
              break;
      }

      c.ContaAnterior = imovel.GetConsumoMesAnterior(c.TipoConta);
      
      Console.Write("Digite a leitura: ");
      c.Leitura = Read.ReadLong();
      
      Console.Write("Digite a data da leitura: (dd/MM/yyyy)");
      c.DataLeitura = Read.ReadDateTime();
      
      
      contaUtil.AdicionarConta(c);
      TelaImovel.GetImovelByMatricula(matricula);
  }
  
  // Cadastrar contas via json
  private static void CadastrarContaViaJson(string matriucla)
  {
    Console.WriteLine("Digite o caminho do arquivo JSON:");
    string filePath = Read.ReadString();

    try
    {
      string jsonData = File.ReadAllText(filePath);
      List<ContaJson>? contas = JsonConvert.DeserializeObject<List<ContaJson>>(jsonData);

      if (contas == null || contas.Count == 0)
      {
        Console.WriteLine("Não foi fornecido nenhum dado válido.");
        return;
      }

      foreach (ContaJson conta in contas)
      {
        Imovel? imovel = imovelUtil.GetImovelByMatricula(matriucla);

        if(imovel == null)
        {
          Console.WriteLine("Houve um erro ao cadastrar! Imovél não encontrado.");
          return;
        }

        contaUtil.AdicionarConta(new Conta(imovel, new Conta(conta.IdContaAnterior), conta.Leitura, conta.DataLeitura, conta.TipoConta));
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("Ocorreu um erro ao processar o arquivo: " + ex.Message);
    }

    TelaImovel.GetImovelByMatricula(matriucla);
  }
}