using trabalhopoo.models;

namespace trabalhopoo.util;

public static class CalculaValorConta
{
  public static double Calcular(Conta c, bool comImpostos)
  {
    switch (c.TipoConta)
    {
      case TipoConta.Agua:
      {
        double total = CalcularAgua(c);

        if (comImpostos)
        {
          int percent = ContaAgua.PERCENT_COFINS;
          total += total * (percent / 100.00);
        }

        return total;
      }
      
      case TipoConta.Luz:
        return CalcularEnergia(c, comImpostos);
        
      default:
        throw new ApplicationException("Tipo de conta não encontrado.");
    }
  }

  private static double CalcularAgua(Conta c)
  {
    switch (c.Imovel.Tipo)
    {
      case TipoImovel.ResidencialSocial:
        return CalcularAguaResidencialSocial(c);
      
      case TipoImovel.Residencial:
        return CalcularAguaResidencial(c);
      
      case TipoImovel.Comercial:
        return CalcularAguaComercial(c);
      
      default: 
        throw new ApplicationException("Tipo de imóvel não encontrado.");
    }
  }
  
  private static double CalcularAguaResidencialSocial(Conta c)
  {
    if (c.GetConsumo() >= 0 && c.GetConsumo() < 6)
      return 10.08 + 5.05;

    if (c.GetConsumo() >= 6 && c.GetConsumo() < 10)
      return (2.241 * c.GetConsumo()) + (1.122 * c.GetConsumo());

    return CalcularAguaResidencial(c);
  }
  
  private static double CalcularAguaResidencial(Conta c)
  {
    if (c.GetConsumo() >= 10 && c.GetConsumo() < 15)
      return (5.447 * c.GetConsumo()) + (2.724 * c.GetConsumo());

    if (c.GetConsumo() >= 15 && c.GetConsumo() < 20)
      return (5.461 * c.GetConsumo()) + (2.731 * c.GetConsumo());

    if (c.GetConsumo() >= 20 && c.GetConsumo() < 40)
      return (5.487 * c.GetConsumo()) + (2.744 * c.GetConsumo());
    
    return (10.066 * c.GetConsumo()) + (5.035 * c.GetConsumo());
  }
  
  private static double CalcularAguaComercial(Conta c)
  {
    if (c.GetConsumo() >= 0 && c.GetConsumo() < 6)
      return 25.79 + 12.90;

    if (c.GetConsumo() >= 6 && c.GetConsumo() < 10)
      return (4.299 * c.GetConsumo()) + (2.149 * c.GetConsumo());
    
    if (c.GetConsumo() >= 10 && c.GetConsumo() < 40)
      return (8.221 * c.GetConsumo()) + (4.111 * c.GetConsumo());
    
    if (c.GetConsumo() >= 40 && c.GetConsumo() < 100)
      return (8.288 * c.GetConsumo()) + (4.144 * c.GetConsumo());
    
    return (8.329 * c.GetConsumo()) + (4.165 * c.GetConsumo());
  }
  
  private static double CalcularEnergia(Conta c, bool comImpostos)
  {
    try
    {
      ContaEnergia? ce = ContaEnergia.Instance(c);

      if ((ce != null) && ce.Imovel.Tipo == TipoImovel.Residencial || ce.Imovel.Tipo == TipoImovel.Comercial)
      {
        double total = ((ce.GetConsumo() * ce.Imovel.Tipo.GetValorTarifaEnergia()) + ContaEnergia.ILUMINACAO_PUBLICA);

        if (ce.Imovel.Tipo == TipoImovel.Residencial && ce.TemDireitoIsencaoImposto())
          return total;

        if (comImpostos)
          total += total * (ContaEnergia.ILUMINACAO_PUBLICA / 100.00);

        return total;
      }

      return 0;
    }
    catch (Exception e)
    {
      return 0;
    }
  }
}