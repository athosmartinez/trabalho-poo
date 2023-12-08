using System.Net.Http.Headers;
using Microsoft.IdentityModel.Tokens;
using trabalhopoo.exception;
using trabalhopoo.models.interfaces;

namespace trabalhopoo.models;

public class Imovel
{
  private long id;
  private string matricula;
  private TipoImovel tipo;
  private Cliente dono;
  private List<Conta> contas;

  public Imovel()
  {
    
  }

  public Imovel(long id)
  {
    this.id = id;
  }

  public Imovel(string matricula, TipoImovel tipo)
  {
    this.matricula = matricula;
    this.tipo = tipo;
  }

  public Imovel(long id, string matricula, TipoImovel tipo, Cliente dono, List<Conta> contas)
  {
    this.id = id;
    this.matricula = matricula;
    this.tipo = tipo;
    this.dono = dono;
    this.contas = contas;
  }

  public long Id
  {
    get => id;
    set => id = value;
  }

  public string Matricula
  {
    get => matricula;
    set => matricula = value;
  }

  public TipoImovel Tipo
  {
    get => tipo;
    set => tipo = value;
  }

  public Cliente Dono
  {
    get => dono;
    set => dono = value;
  }

  public List<Conta> Contas
  {
    get => contas;
    set => contas = value;
  }

  public Conta? GetConsumoMesAnterior(TipoConta tipoConta)
  {
    DateTime data = DateTime.Now;
    data = data.AddMonths(-1);
    
    return GetConta(data, tipoConta);
  }

  public Conta? GetContaAtual(TipoConta tipoConta)
  {
    return GetConta(DateTime.Now, tipoConta);
  }

  public Conta? GetConta(DateTime mes, TipoConta tipoConta) 
  {
    foreach (Conta c in this.contas)
    {
      if (c.TipoConta == tipoConta && c.DataLeitura.Month == mes.Month && c.DataLeitura.Year == mes.Year)
        return c;
    }

    return null;
  }
  
  public long GetVariacaoConsumo(TipoConta tipoConta, DateTime mes1, DateTime mes2)
  {
    return (long) GetVariacao(tipoConta, mes1, mes2, true);
  }
  
  public double GetVariacaoValor(TipoConta tipoConta, DateTime mes1, DateTime mes2) {
    return (double) GetVariacao(tipoConta, mes1, mes2, false);
  }

  private object GetVariacao(TipoConta tipoConta, DateTime mes1, DateTime mes2, bool isConsumo)
  {
    if (this.contas.IsNullOrEmpty())
      return 0;
    
    Conta? cMes1 = this.GetConta(mes1, tipoConta);
    Conta? cMes2 = this.GetConta(mes2, tipoConta);

    if (cMes1 == null)
      throw new ContaException("Não existe conta para o mês " + mes1.ToString("MM/yyyy"));

    if (cMes2 == null)
      throw new ContaException("Não existe conta para o mês " + mes2.ToString("MM/yyyy"));

    if (isConsumo)
      return cMes1.GetConsumo() - cMes2.GetConsumo();
    
    
    if (tipoConta == TipoConta.Agua)
    {
      ContaAgua? c1 = ContaAgua.Instance(cMes1);
      ContaAgua? c2 = ContaAgua.Instance(cMes2);
      
      if (c1 != null && c2 != null) return c1.GetTotalConta() - c2.GetTotalConta();
    } 
    else if (tipoConta == TipoConta.Luz)
    {
      ContaEnergia? c1 = ContaEnergia.Instance(cMes1);
      ContaEnergia? c2 = ContaEnergia.Instance(cMes2);
      
      if (c1 != null && c2 != null) return c1.GetTotalConta() - c2.GetTotalConta();
    }
    
    return 0;
  }
  
  public DateTime? GetMesMaiorConsumo(TipoConta tipoConta)
  {
    return GetMesMaior(tipoConta, true);
  }
  
  public DateTime? GetMesMaiorGasto(TipoConta tipoConta) {
    return GetMesMaior(tipoConta, false);
  }

  private DateTime? GetMesMaior(TipoConta tipoConta, bool isConsumo)
  {
    if (this.contas.IsNullOrEmpty())
      return null;
    
    Conta maior = new Conta();

    foreach (Conta c in this.contas.Where(c => c.TipoConta == tipoConta))
    {
      if (isConsumo)
      {
        if (c.GetConsumo() > maior.GetConsumo())
          maior = c;
      }
      else
      {
          IConta? conta = (IConta) ContaAgua.Instance(c);
          IConta? contaMaior = (IConta) ContaAgua.Instance(maior);
          if (conta != null && contaMaior != null && conta.GetTotalConta() > contaMaior.GetTotalConta()) maior = c;
      }
    }
    
    return maior.DataLeitura;  
  }

  public double GetConsumoMedio(TipoConta tipoConta)
  {
    return CalculaMedia(tipoConta, true); 
  }
  
  public double GetValorMedio(TipoConta tipoConta) 
  {
    return CalculaMedia(tipoConta, false); 
  }

  private double CalculaMedia(TipoConta tipoConta, bool isConsumo)
  {
    if (this.contas.IsNullOrEmpty())
      return 0;
    
    double soma = 0;
    int count = 0;
    
    foreach (Conta c in this.contas.Where(c => c.TipoConta == tipoConta))
    {
      count++;
      if (isConsumo)
      {
        soma += c.GetConsumo();
      } 
      else
      {
        if (tipoConta == TipoConta.Agua)
        {
          ContaAgua? conta = ContaAgua.Instance(c);
          if (conta != null) soma += conta.GetTotalConta();
        } 
        else if (tipoConta == TipoConta.Luz)
        {
          ContaEnergia? conta = ContaEnergia.Instance(c);
          if (conta != null) soma += conta.GetTotalConta();
        }
      }
    }
    
    return soma / count;
  }
}