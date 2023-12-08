namespace trabalhopoo.json;

public class ImovelJson
{
  public string matricula;
  public string tipo;

  public ImovelJson()
  {
            
  }

  public ImovelJson(string matricula, string tipo)
  {
    this.matricula = matricula;
    this.tipo = tipo;
  }

  public string Matricula
  {
    get => matricula;
    set => matricula = value;
  }

  public string Tipo
  {
    get => tipo;
    set => tipo = value;
  }
}