using System;
using System.Collections.Generic;
using System.Text;

namespace CONSOLE_CON_UNI_DOTNET_SOAP.Views
{
  using CatalogServiceRef;
  using CONSOLE_CON_UNI_DOTNET_SOAP.Controllers;
  using CONSOLE_CON_UNI_DOTNET_SOAP.Models.Enums;
  using CONSOLE_CON_UNI_DOTNET_SOAP.Views.Utils;
  using ConverterServiceRef;
  using System;
  using System.Collections.Generic;
  using System.Globalization;

  internal class ConverterView
  {
    public static void ShowTypes()
    {
      MessagesUtils.PrintMessage("1. Área");
      MessagesUtils.PrintMessage("2. Longitud");
      MessagesUtils.PrintMessage("3. Masa / Peso");
      MessagesUtils.PrintMessage("4. Temperatura");
      MessagesUtils.PrintMessage("5. Tiempo");
      MessagesUtils.PrintMessage("6. Volumen");
    }

    public static TypesEnum SelectType()
    {
      MessagesUtils.PrintMessage("Seleccione el tipo de conversión a realizar:");

      ShowTypes();

      MessagesUtils.PrintMessage("Opción: ");

      string option = (Console.ReadLine() ?? string.Empty)
        .Trim()
        .ToLowerInvariant();

      switch (option)
      {
        case "1":
        case "area":
          return TypesEnum.Area;

        case "2":
        case "longitud":
          return TypesEnum.Longitud;

        case "3":
        case "masa":
        case "peso":
          return TypesEnum.Masa;

        case "4":
        case "temperatura":
          return TypesEnum.Temperatura;

        case "5":
        case "tiempo":
          return TypesEnum.Tiempo;

        case "6":
        case "volumen":
          return TypesEnum.Volumen;

        default:
          MessagesUtils.PrintMessage("Opción no válida. Saliendo.");

          return TypesEnum.None;
      }
    }

    public static void ShowCatalog()
    {
      TypesEnum type = SelectType();

      if (type == TypesEnum.None)
      {
        return;
      }

      CatalogController ctrl = new CatalogController(type);

      CatalogResponseDTO? dto = ctrl.GetCatalog();

      if (dto?.estado == CatalogServiceRef.EstadosEnum.ERROR)
      {
        return;
      }

      MessagesUtils.PrintMessage(dto?.mensaje ?? string.Empty);

      List<string> res = dto?.resultado ?? [];

      foreach (string unidad in res)
      {
        MessagesUtils.PrintMessage(unidad);
      }
    }

    public static void ShowConversor()
    {
      TypesEnum type = SelectType();

      if (type == TypesEnum.None)
      {
        return;
      }

      CatalogController ctrl = new CatalogController(type);

      CatalogResponseDTO? catalogDto = ctrl.GetCatalog();

      if (catalogDto?.estado == CatalogServiceRef.EstadosEnum.ERROR)
      {
        return;
      }

      MessagesUtils.PrintMessage(catalogDto?.mensaje ?? string.Empty);

      List<string> unidades = catalogDto?.resultado ?? [];

      foreach (string unidad in unidades)
      {
        MessagesUtils.PrintMessage(unidad);
      }

      double value;

      try
      {
        MessagesUtils.PrintMessage("Ingresa el valor a convertir: ");

        string input = Console.ReadLine() ?? string.Empty;

        value = double.Parse(
            input.Trim(),
            CultureInfo.InvariantCulture
        );
      }
      catch (FormatException)
      {
        MessagesUtils.PrintMessage("Error: ingresa un valor numérico válido.");

        return;
      }
      catch (OverflowException)
      {
        MessagesUtils.PrintMessage("Error: el valor ingresado está fuera de rango.");

        return;
      }

      MessagesUtils.PrintMessage("Ingresa la unidad inicial: ");
      string from = Console.ReadLine() ?? string.Empty;

      MessagesUtils.PrintMessage("Ingresa la unidad final: ");
      string to = Console.ReadLine() ?? string.Empty;

      ConversorController conCtrl = new ConversorController(type);

      ConverterResponseDTO? dto = conCtrl.MakeConversion(value, from, to);

      MessagesUtils.PrintMessage(
          dto?.estado == ConverterServiceRef.EstadosEnum.OK
              ? dto.mensaje ?? string.Empty
              : $"ERROR: {dto?.mensaje}");

      MessagesUtils.PrintMessage(
          Convert.ToString(dto?.resultado, CultureInfo.InvariantCulture)
          ?? string.Empty);
    }

    public static void ShowMenu()
    {
      bool keepInMenu = true;

      while (keepInMenu)
      {
        MessagesUtils.PrintTitle("Menú");

        MessagesUtils.PrintMessage("¿Qué desea realizar?");
        MessagesUtils.PrintMessage("1. Conversión de unidades");
        MessagesUtils.PrintMessage("2. Ver Catálogo");
        MessagesUtils.PrintMessage("3. Salir");
        MessagesUtils.PrintMessage("Opción: ");

        string input = Console.ReadLine() ?? string.Empty;

        if (!int.TryParse(input.Trim(), out int option))
        {
          MessagesUtils.PrintMessage("Error: ingresa un valor entero válido.");

          continue;
        }

        switch (option)
        {
          case 1:
            ShowConversor();
            break;

          case 2:
            ShowCatalog();
            break;

          case 3:
            keepInMenu = false;
            break;

          default:
            MessagesUtils.PrintMessage("¡Opción inválida!");
            break;
        }
      }
    }
  }
}
