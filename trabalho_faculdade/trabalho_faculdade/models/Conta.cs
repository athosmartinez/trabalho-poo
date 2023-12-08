namespace trabalhopoo.models;

public class Conta
{
  private long id;
  private Imovel imovel;
  private Conta contaAnterior;
  private long leitura;
  private DateTime dataLeitura;
  private TipoConta tipoConta;

  public Conta()
  {
  }

  public Conta(long id)
  {
    this.id = id;
  }

  public Conta(Imovel imovel, Conta contaAnterior, long leitura, DateTime dataLeitura, TipoConta tipoConta)
  {
    this.imovel = imovel;
    this.contaAnterior = contaAnterior;
    this.leitura = leitura;
    this.dataLeitura = dataLeitura;
    this.tipoConta = tipoConta;
  }

  public long Id
  {
    get => id;
    set => id = value;
  }

  public Imovel Imovel
  {
    get => imovel;
    set => imovel = value;
  }

  public Conta ContaAnterior
  {
    get => contaAnterior;
    set => contaAnterior = value;
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
  
  public long GetConsumo() {
    return this.leitura - (this.contaAnterior != null ? this.contaAnterior.Leitura : 0);
  }
}