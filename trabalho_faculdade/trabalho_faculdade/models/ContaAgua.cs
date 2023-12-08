using trabalhopoo.models.interfaces;
using trabalhopoo.util;

namespace trabalhopoo.models;

public class ContaAgua : Conta, IConta
{
  public static readonly int PERCENT_COFINS = 3; 
  
  public ContaAgua()
  {
  }

  public ContaAgua(long id) : base(id)
  {
  }

  public double GetTotalConta() {
    return CalculaValorConta.Calcular(this, true);
  }

  public double GetTotalContaSemImpostos() {
    return CalculaValorConta.Calcular(this, false);
  }
  
  public static ContaAgua? Instance(Conta? conta)
  {
    ContaAgua ca = new ContaAgua();
    
    if (conta == null)
      return null;
    
    ca.Id = conta.Id;
    ca.Imovel = conta.Imovel;
    ca.ContaAnterior = conta.ContaAnterior;
    ca.Leitura = conta.Leitura;
    ca.DataLeitura = conta.DataLeitura;
    ca.TipoConta  = conta.TipoConta;

    return ca;
  }
}