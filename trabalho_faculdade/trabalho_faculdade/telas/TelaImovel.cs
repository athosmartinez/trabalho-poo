using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using trabalhopoo.db;
using trabalhopoo.json;
using trabalhopoo.models;
using trabalhopoo.util;

namespace trabalhopoo.telas;

public class TelaImovel
{
    // Listar imoveis de um cliente
    public static void listInformacoesScreen(String? cpf)
    {
        Console.WriteLine("---------------------------------------------------------------------------------");
        
        if (string.IsNullOrEmpty(cpf))
        {
            Console.WriteLine("Digite o CPF do cliente que se deseja listar as informações de seus os imóveis:");
            cpf = Read.ReadCpf();
        }
        
        Cliente? cliente = clienteUtil.procuraClienteCpf(cpf);

        Console.WriteLine("---------------------------------------------------------------------------------");
        
        Console.Clear();

        if (cliente == null)
        {
            Console.WriteLine("Clinte não encontrado.");
            Console.WriteLine("\n");
            TelaCliente.listClientScreen();
            return;
        }
        
        if (cliente.Imoveis.Count == 0)
        {
            Console.WriteLine("Nenhum imóvel encontrado.");
            Console.WriteLine("---------------------------------------");
            optionsSemImovel(cliente.Cpf);
            return;
        }
        
        Console.Clear();
        Console.WriteLine("Lista de imóveis do cliente " + Convert.ToString(cliente.Nome));
        Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
        
        string fmtMesAno = DateTime.Now.AddMonths(-1).ToString("MM/yyyy");

        Console.WriteLine("Mat." +
                          "\tTipo" +
                          "\t\tCons. água " +  fmtMesAno +
                          "\tTot. água " + fmtMesAno +
                          "\tTot. s/imposto água " + fmtMesAno +
                          "\tVal. Médio água " +
                          "\tMês maior cons. água " +
                          "\tMês maior val. água " +
                          "\tCons. luz " + fmtMesAno +
                          "\tTot. luz " + fmtMesAno + 
                          "\tTot. s/imposto luz " + fmtMesAno +
                          "\tVal. Médio luz "+
                          "\t\tMês maior cons. luz " + 
                          "\tMês maior val. luz "
        );
        
        Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
        
        foreach (Imovel imovel in cliente.Imoveis)
        {
            ContaAgua? ca = ContaAgua.Instance(imovel.GetConsumoMesAnterior(TipoConta.Agua));
            ContaEnergia? ce = ContaEnergia.Instance(imovel.GetConsumoMesAnterior(TipoConta.Luz));
        
            Console.WriteLine($"{imovel.Matricula}" +
                              $"\t{imovel.Tipo}" +
                              $"\t{(ca == null ? "" : ca.GetConsumo())}" +
                              $"\t\t\t{(ca == null ? "" : "R$ " + ca.GetTotalConta().ToString("0.00"))}" +
                              $"\t\t{(ca == null ? "" : "R$ " + ca.GetTotalContaSemImpostos().ToString("0.00"))}" +
                              $"\t\t\tR$ {imovel.GetValorMedio(TipoConta.Agua):0.00}" +
                              $"\t\t{imovel.GetMesMaiorConsumo(TipoConta.Agua)?.ToString("MM/yyyy")}" +
                              $"\t\t\t{imovel.GetMesMaiorGasto(TipoConta.Agua)?.ToString("MM/yyyy")}" +
                              $"\t\t\t{(ce == null ? "" : ce.GetConsumo())}" +
                              $"\t\t\t{(ce == null ? "" : "R$ " + ce.GetTotalConta().ToString("0.00"))}" +
                              $"\t\t{(ce == null ? "" : "R$ " + ce.GetTotalContaSemImpostos().ToString("0.00"))}" +
                              $"\t\t\tR$ {imovel.GetValorMedio(TipoConta.Luz):0.00}" +
                              $"\t\t{imovel.GetMesMaiorConsumo(TipoConta.Luz)?.ToString("MM/yyyy")}" +
                              $"\t\t\t{imovel.GetMesMaiorGasto(TipoConta.Luz)?.ToString("MM/yyyy")}"
            );
        }

        Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
        optionsListagemImovel(cliente.Cpf);
    }

    private static void optionsSemImovel(string cpf)
    {
        Console.WriteLine("Escolha a opção:");
        Console.WriteLine("1. Cadastrar novo imovel para o cliente");
        Console.WriteLine("2. Voltar ao inicio");
        Console.Write("Escolha uma opção (1 ou 2): ");

        string op = Read.ReadOptions(new[] { "1", "2" });
        
        switch (op)
        {
            case "1":
                newImovelScreen(cpf);
                break;
            case "2":
                Console.Clear();
                TelaCliente.listClientScreen();
                break;
            default:
                Console.WriteLine("Opção inválida.");
                break;
        }
    }

    private static void optionsListagemImovel(string cpf)
    {
        Console.WriteLine("Escolha a opção:");
        Console.WriteLine("1. Cadastrar novo imovel para o cliente");
        Console.WriteLine("2. Ver dados do imóvel");
        Console.WriteLine("3. Ver variação de consumo entre dois meses");
        Console.WriteLine("4. Voltar ao inicio");
        Console.Write("Escolha uma opção (1 , 2, 3 ou 4): ");

        string op = Read.ReadOptions(new[] { "1", "2", "3", "4" });
        
        switch (op)
        {
            case "1":
                newImovelScreen(cpf);
                break;
            case "2":
                GetImovelByMatricula(null);
                break;
            case "3":
                GetVariacaoContas(cpf);
                break;
            case "4":
                Console.Clear();
                TelaCliente.listClientScreen();
                break;
            default:
                Console.WriteLine("Opção inválida.");
                break;
        }
    }

    public static void GetVariacaoContas(string cpf)
    {
        Console.WriteLine("Digite o tipo da conta que deseja verificar a variação:");
        Console.WriteLine("1. Água");
        Console.WriteLine("2. Luz");

        string op = Read.ReadOptions(new[] { "1", "2" });
        
        switch (op)
        {
            case "1":
                GetVariacaoContas(TipoConta.Agua, cpf);
                break;
            case "2":
                GetVariacaoContas(TipoConta.Luz, cpf);
                break;
            default:
                Console.WriteLine("Tipo inválido.");
                break;
        }
    }

    public static void GetVariacaoContas(TipoConta tipo, string cpf)
    {
        Console.WriteLine("Informe a matricula do imovel que deseja consultar a variacção:");
        string matricula = Read.ReadStringMax(20);
        
        Console.WriteLine("Informe mês e ano inicial (MM/yyyy):");
        DateTime? dataInicial = Read.ReadDate();
        
        Console.WriteLine("Informe mês e ano final (MM/yyyy):");
        DateTime? dataFinal = Read.ReadDate();
        
        if (dataInicial == null || dataFinal == null)
        {
            Console.WriteLine("Datas inválidas. Tente denovo");
            GetVariacaoContas(tipo, cpf);
            return;
        }
        
        Imovel? imovel = imovelUtil.GetImovelByMatricula(matricula);
        
        if (imovel == null)
        {
            Console.WriteLine("Imovel não encontrado.");
            listInformacoesScreen(cpf);
            return;
        }

        Console.WriteLine("A variação do consumo entre os meses informdos foi de: " + imovel.GetVariacaoConsumo(tipo, dataInicial.Value, dataFinal.Value));
        Console.WriteLine("A variação do gasto entre os meses informdos foi de: R$ " + imovel.GetVariacaoValor(tipo, dataInicial.Value, dataFinal.Value).ToString("0.00"));
        
        Console.WriteLine("Aperte 1 para voltar a listagem de imoveis");
        
        string op = Read.ReadOptions(new[] { "1" });

        if (op == "1")
        {
            listInformacoesScreen(cpf);
            return;
        }
        
        TelaCliente.listClientScreen();
    }

    public static void GetImovelByMatricula(string? matricula)
    {
        if (matricula.IsNullOrEmpty())
        {
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.WriteLine("Digite a matricula do imovel que se deseja ver as informações:");
            matricula = Read.ReadStringMax(20);
        }
        
        Imovel? imovel = imovelUtil.GetImovelByMatricula(matricula);

        Console.WriteLine("---------------------------------------------------------------------------------");

        if (imovel == null)
        {
            Console.WriteLine("Imovel não encontrado.");
            return;
        }
        
        Console.Clear();
        Console.Write("\n");
        Console.WriteLine("Dados do imovel de matrícula: " + matricula);
        Console.WriteLine("-----------------------------------------------------");
        Console.WriteLine("Id\tMatrícula\tTipo");
        Console.WriteLine($"{imovel.Id}\t{imovel.Matricula}\t\t{imovel.Tipo}");
        Console.WriteLine("-----------------------------------------------------");
        Console.WriteLine("\n");
        Console.WriteLine("Lista de contas do imovel");
        Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");
        
        if (imovel.Contas == null || imovel.Contas.Count == 0)
        {
            Console.WriteLine("Nenhuma conta encontrada.");
        }
        else
        {
            Console.WriteLine("Data da Leitura" +
                              "\t\tLeitura" +
                              "\t\tTipo da conta" +
                              "\t\tConsumo" +
                              "\t\tTotal" +
                              "\t\tTotal sem imposto"
            );
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");
            
            foreach (Conta c in imovel.Contas)
            {
                ContaEnergia? ce = null;
                ContaAgua? ca = null;
                
                if (c.TipoConta == TipoConta.Luz)
                    ce = ContaEnergia.Instance(c);
                
                if (c.TipoConta == TipoConta.Agua)
                    ca = ContaAgua.Instance(c);
            
                Console.WriteLine($"{c.DataLeitura.ToString("d")}" +
                                  $"\t\t{c.Leitura}" +
                                  $"\t\t{c.TipoConta}" +
                                  $"\t\t\t{c.GetConsumo()}" +
                                  $"\t\t{(c.TipoConta == TipoConta.Luz && ce != null ? "R$ " + ce.GetTotalConta().ToString("0.00") : (c.TipoConta == TipoConta.Agua && ca != null ? "R$ " + ca.GetTotalConta().ToString("0.00") : ""))}" +
                                  $"\t{(c.TipoConta == TipoConta.Luz && ce != null ? "R$ " + ce.GetTotalContaSemImpostos().ToString("0.00") : (c.TipoConta == TipoConta.Agua && ca != null ? "R$ " + ca.GetTotalContaSemImpostos().ToString("0.00") : ""))}"
                );
            }
            
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");
        }

        TelaConta.NewContaControle(imovel.Matricula);
    }
    
    // Opções cadastro de imovel
    public static void newImovelScreen(string cpf)
    {
        Console.Clear();
        Console.WriteLine("Tela cadastrar imovel");
        Console.WriteLine("---------------------------------------");
        Console.WriteLine("Escolha a opção:");
        Console.WriteLine("1. Inserir imovel manualmente");
        Console.WriteLine("2. Inserir imovel(s) via arquivo JSON");
        Console.Write("Escolha uma opção (1 ou 2): ");

        string op = Read.ReadOptions(new[] { "1", "2" });
        
        switch (op)
        {
            case "1":
                CadastrarImovelManualmente(cpf);
                break;
            case "2":
                CadastrarImovelViaJson(cpf);
                break;
            default:
                Console.WriteLine("Opção inválida.");
                break;
        }
    }

    // Cadastrar imovel manualmente
    private static void CadastrarImovelManualmente(string cpf)
    {
        long idCliente = clienteUtil.procuraIdClienteCpf(cpf);
        
        Console.WriteLine("Digite a matrícula do imóvel:");
        string matricula = Read.ReadStringMax(20);

        Console.WriteLine("Digite o tipo do imóvel:");
        Console.WriteLine("1. Comercial");
        Console.WriteLine("2. ResidencialSocial");
        Console.WriteLine("3. Residencial");
        
        TipoImovel tipo = TipoImovel.Residencial; // Valor padrão
        
        string op = Read.ReadOptions(new[] { "1", "2", "3" });
        
        switch (op)
        {
            case "1":
                tipo = TipoImovel.Comercial;
                break;
            case "2":
                tipo = TipoImovel.ResidencialSocial;
                break;
            case "3":
                tipo = TipoImovel.Residencial;
                break;
            default:
                Console.WriteLine("Tipo inválido. Usando 'Residencial' como padrão.");
                break;
        }
        
        if(idCliente == 0)
        {
            Console.WriteLine("Houve um erro ao cadastrar! Cliente não encontrado.");
            return;
        }

        if (string.IsNullOrEmpty(matricula) || matricula.Length > 20)
        {
            Console.WriteLine("Houve um erro ao cadastrar! Matrícula inválida.");
            return;
        }
        
        Imovel imovel = new Imovel(matricula, tipo);
        
        imovelUtil.AdicionarImovel(imovel, idCliente);
        listInformacoesScreen(cpf);
    }

    // Cadastrar imoveis via json
    private static void CadastrarImovelViaJson(string cpf)
    {
        Console.WriteLine("Digite o caminho do arquivo JSON:");
        string filePath = Read.ReadString();

        try
        {
            string jsonData = File.ReadAllText(filePath);
            List<ImovelJson>? imoveis = JsonConvert.DeserializeObject<List<ImovelJson>>(jsonData);

            if (imoveis == null || imoveis.Count == 0)
            {
                Console.WriteLine("Não foi fornecido nenhum dado válido.");
                return;
            }

            foreach (ImovelJson imovelJson in imoveis)
            {
                long idCliente = clienteUtil.procuraIdClienteCpf(cpf);

                if(idCliente == 0 || string.IsNullOrEmpty(imovelJson.Matricula))
                {
                    Console.WriteLine($"Operação cancelada para o imóvel com matrícula {imovelJson.Matricula}.");
                    continue;
                }

                Imovel imovel = new Imovel
                {
                    Matricula = imovelJson.Matricula,
                    Tipo = (TipoImovel)Enum.Parse(typeof(TipoImovel), imovelJson.Tipo)
                };

                imovelUtil.AdicionarImovel(imovel, idCliente);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ocorreu um erro ao processar o arquivo: " + ex.Message);
        }
    }
}