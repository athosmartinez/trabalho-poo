using trabalhopoo.models.interfaces;
using trabalhopoo.util;

namespace trabalhopoo.models;

public class ContaEnergia : Conta, IConta
{
  public static readonly double ILUMINACAO_PUBLICA = 13.25; 
  
  public ContaEnergia()
  {
  }

  public ContaEnergia(long id) : base(id)
  {
  }

  public double GetTotalConta()
  {
    return CalculaValorConta.Calcular(this, true);
  }

  public double GetTotalContaSemImpostos()
  {
    return CalculaValorConta.Calcular(this, false);
  }

  public bool TemDireitoIsencaoImposto()
  {
    return this.GetConsumo() < 90;
  }

  public static ContaEnergia? Instance(Conta? conta)
  {
    ContaEnergia ce = new ContaEnergia();
    
    if (conta == null)
      return null;
    
    ce.Id = conta.Id;
    ce.Imovel = conta.Imovel;
    ce.ContaAnterior = conta.ContaAnterior;
    ce.Leitura = conta.Leitura;
    ce.DataLeitura = conta.DataLeitura;
    ce.TipoConta  = conta.TipoConta;

    return ce;
  }
}