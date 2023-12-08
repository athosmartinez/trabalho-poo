namespace trabalhopoo.models {
  public class Cliente {
    private long id;
    private string nome;
    private string cpf;
    private List<Imovel> imoveis;

    public Cliente() {

    }

    public Cliente(long id)
    {
      this.id = id;
    }

    public Cliente(string cpf)
    {
      this.cpf = cpf;
    }

    public Cliente(string nome, string cpf)
    {
      this.nome = nome;
      this.cpf = cpf;
    }

    public Cliente(long id, string nome, string cpf, List<Imovel> imoveis) {
      this.id = id;
      this.nome = nome;
      this.cpf = cpf;
      this.imoveis = imoveis;
    }

    public long Id
    {
      get => id; 
      set => id = value;
    }

    public string Nome
    {
      get => nome; 
      set => nome = value;
    }

    public List<Imovel> Imoveis
    {
      get => imoveis; 
      set => imoveis = value;
    }

    public string Cpf
    {
      get => cpf;
      set => cpf = value;
    }

    public Imovel? GetImovelPorMatricula(string matricula) {
      foreach (Imovel imovel in imoveis) {
        if (imovel.Matricula == matricula) {
          return imovel;
        }
      }
      
      return null;
    }
  }
}
