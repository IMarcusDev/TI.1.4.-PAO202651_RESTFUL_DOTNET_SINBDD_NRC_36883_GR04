using Microsoft.Extensions.Logging;
using MOV_CON_UNI_DOTNET_RESTFUL.Config;
using MOV_CON_UNI_DOTNET_RESTFUL.Controllers;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Interface;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Repositories;
using MOV_CON_UNI_DOTNET_RESTFUL.Services;
using MOV_CON_UNI_DOTNET_RESTFUL.Views;

namespace MOV_CON_UNI_DOTNET_RESTFUL
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<SessionService>();

            builder.Services.AddSingleton(_ => new ClienteRest(RestConfig.BaseUrl));

            builder.Services.AddSingleton<ILoginRepository, LoginRepository>();
            builder.Services.AddSingleton<ICatalogRepository, CatalogRepository>();
            builder.Services.AddSingleton<IConverterRepository, ConvertRepository>();

            builder.Services.AddTransient<LoginController>();
            builder.Services.AddTransient<ConversionController>();
            builder.Services.AddTransient<CatalogController>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ConversionPage>();
            builder.Services.AddTransient<CatalogPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("SinSubrayado", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("SinSubrayado", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.BackgroundTintList =
                    Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });

            return builder.Build();
        }
    }
}
