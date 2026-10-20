
using CONSOLE_CON_UNI_DOTNET_SOAP.Views;
using CONSOLE_CON_UNI_DOTNET_SOAP.Views.Utils;

// Splash Art
MessagesUtils.PrintMonster();
Thread.Sleep(TimeSpan.FromSeconds(3));

// Login
bool login = LoginView.UserHasLogin();

if (!login)
{
  return;
}

// Menu
ConverterView.ShowMenu();

MessagesUtils.PrintMessage("Hasta Luego!");
Thread.Sleep(TimeSpan.FromSeconds(3));

