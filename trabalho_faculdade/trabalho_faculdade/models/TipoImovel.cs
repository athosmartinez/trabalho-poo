namespace trabalhopoo.models;

public enum TipoImovel
{
  Comercial = 0,
  Residencial = 1,
  ResidencialSocial = 2,
}

public static class TipoImovelExtensions
{
  public static double GetValorTarifaEnergia(this TipoImovel tipo)
  {
    switch (tipo)
    {
      case TipoImovel.Comercial:
        return 0.41;
      case TipoImovel.Residencial:
        return 0.46;
      default:
        throw new ApplicationException("Valor de enum informado naõ possui tarifa de energia.");
    }
  }
  
  public static double GetPercentImpostoEnergia(this TipoImovel tipo)
  {
    switch (tipo)
    {
      case TipoImovel.Comercial:
        return 21.95;
      case TipoImovel.Residencial:
        return 42.85;
      default:
        throw new ApplicationException("Valor de enum informado naõ possui imposto de energia.");
    }
  }
}