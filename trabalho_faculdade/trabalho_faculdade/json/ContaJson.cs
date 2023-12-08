using trabalhopoo.models;

namespace trabalhopoo.json;

public class ContaJson
{
  private long idContaAnterior;
  private long leitura;
  private DateTime dataLeitura;
  private TipoConta tipoConta;
  
  public ContaJson()
  {
  }

  public ContaJson(long idContaAnterior, long leitura, DateTime dataLeitura, TipoConta tipoConta)
  {
    this.idContaAnterior = idContaAnterior;
    this.leitura = leitura;
    this.dataLeitura = dataLeitura;
    this.tipoConta = tipoConta;
  }

  public long IdContaAnterior
  {
    get => idContaAnterior;
    set => idContaAnterior = value;
  }

  public long Leitura
  {
    get => leitura;
    set => leitura = value;
  }

  public DateTime DataLeitura
  {
    get => dataLeitura;
    set => dataLeitura = value;
  }

  public TipoConta TipoConta
  {
    get => tipoConta;
    set => tipoConta = value;
  }
}