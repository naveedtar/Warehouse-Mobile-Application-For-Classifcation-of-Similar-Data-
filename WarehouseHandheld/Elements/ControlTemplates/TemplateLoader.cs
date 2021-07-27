using System;
using Xamarin.Forms;
namespace WarehouseHandheld.Elements.ControlTemplates
{
    public class TemplateLoader
    {
        public static string MainControlTemplate = "MainControlTemplate";

        public static void Init()
        {
            MainControlTemplate template = new MainControlTemplate();

            if (Application.Current.Resources == null)
            {
                Application.Current.Resources = new ResourceDictionary();
            }

            ControlTemplate mainTemplate = new ControlTemplate(typeof(MainControlTemplate));
            Application.Current.Resources.Add(MainControlTemplate, mainTemplate);
        }
    }
}
