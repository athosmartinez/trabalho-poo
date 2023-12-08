namespace trabalhopoo.json;

public class ClienteJson
{
  private string nome;
  private string cpf;
  
  public ClienteJson()
  {
  }
  
  public ClienteJson(string nome, string cpf)
  {
    this.nome = nome;
    this.cpf = cpf;
  }

  public string Nome
  {
    get => nome;
    set => nome = value;
  }

  public string Cpf
  {
    get => cpf;
    set => cpf = value;
  }
}