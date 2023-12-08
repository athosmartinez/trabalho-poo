using Microsoft.IdentityModel.Tokens;

namespace trabalhopoo.util;

public static class Read
{
  public static string ReadCpf() {
    bool isValid = false;
    string? value = "";

    while (!isValid) {
      try
      {
        value = Console.ReadLine();
        
        if (value == null || value.Length != 11)
          throw new ApplicationException("O CPF deve conter 11 dígitos");
        
        isValid = true;
      }
      catch (Exception e) {
        Console.WriteLine("Erro! Informe o cpf novamente. " + e.Message);
      }
    }

    return value;
  }
  
  public static string ReadOptions(string[] options) {
    bool isValid = false;
    string? value = "";

    while (!isValid) {
      try
      {
        value = Console.ReadLine();
        
        if (value == null || !options.Contains(value))
          throw new ApplicationException("Valor inválido! Informe outro valor");
        
        isValid = true;
      }
      catch (Exception e) {
        Console.WriteLine(e.Message);
      }
    }

    return value;
  }
  
  public static string ReadStringMax(int max) {
    bool isValid = false;
    string? value = "";

    while (!isValid) {
      try
      {
        value = Console.ReadLine();
        
        if (value == null || value.Length == 0 || value == "" )
          throw new ApplicationException("Necessário informar um valor válido!");
        
        if (value.Length > max)
          throw new ApplicationException("Tamanho inválido! Só é permitido até " + max + " caracteres");
        
        isValid = true;
      }
      catch (Exception e) {
        Console.WriteLine(e.Message);
      }
    }

    return value;
  }
  
  public static string ReadString() {
    bool isValid = false;
    string? value = "";

    while (!isValid) {
      try
      {
        value = Console.ReadLine();
        
        if (value == null || value.Length == 0 || value == "" )
          throw new ApplicationException("Necessário informar um valor válido!");
        
        isValid = true;
      }
      catch (Exception e) {
        Console.WriteLine(e.Message);
      }
    }

    return value;
  }
  
  public static long ReadLong() {
    bool isValid = false;
    long value = -1;

    while (!isValid) {
      try
      {
        value = Convert.ToInt64(Console.ReadLine());
        
        if (value == null || value < 0)
          throw new ApplicationException("Necessário informar um valor inteiro positivo válido!");
        
        isValid = true;
      }
      catch (Exception e) {
        Console.WriteLine(e.Message);
      }
    }

    return value;
  }
  
  public static DateTime ReadDateTime() {
    bool isValid = false;
    DateTime value = DateTime.Now;

    while (!isValid) {
      try
      {
        value = Convert.ToDateTime(Console.ReadLine());
        
        if (value == null)
          throw new ApplicationException("Necessário informar uma data válida!");
        
        isValid = true;
      }
      catch (Exception e) {
        Console.WriteLine(e.Message);
      }
    }

    return value;
  }

  public static DateTime? ReadDate()
  {
    bool isValid = false;
    string? value = "";
    DateTime? result = null;

    while (!isValid)
    {
      try
      {
        value = Console.ReadLine();

        if (value.IsNullOrEmpty())
          throw new ApplicationException("Necessário informar uma data válida!");
        
        string[] values = value.Split("/");

        result = new DateTime(Convert.ToInt32(values[1]), Convert.ToInt32(values[0]), DateTime.Now.Day);

        isValid = true;
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }

    return result;
  }
}