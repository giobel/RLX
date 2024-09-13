#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Media.Imaging;


#endregion

namespace RLX
{
    internal class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {

            string tabName = "RLX";

            try
            {
                a.CreateRibbonTab(tabName);

                RibbonPanel explodeFamilies = GetSetRibbonPanel(a, tabName, "Explode Families");

                if (AddPushButton(explodeFamilies, "btnWallSplitter", "Hydrant Explode", "", "", "RLX.HydrantExplode", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button Split Wall", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch { }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }


        private RibbonPanel GetSetRibbonPanel(UIControlledApplication application, string tabName, string panelName)
        {
            List<RibbonPanel> tabList = new List<RibbonPanel>();

            tabList = application.GetRibbonPanels(tabName);

            RibbonPanel tab = null;

            foreach (RibbonPanel r in tabList)
            {
                if (r.Name.ToUpper() == panelName.ToUpper())
                {
                    tab = r;
                }
            }

            if (tab is null)
                tab = application.CreateRibbonPanel(tabName, panelName);

            return tab;
        }

        private Boolean AddPushButton(RibbonPanel Panel, string ButtonName, string ButtonText, string ImagePath16, string ImagePath32, string dllClass, string Tooltip)
        {

            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            try
            {
                PushButtonData m_pbData = new PushButtonData(ButtonName, ButtonText, thisAssemblyPath, dllClass);

                if (ImagePath16 != "")
                {
                    try
                    {
                        m_pbData.Image = new BitmapImage(new Uri(ImagePath16));
                    }
                    catch
                    {
                        //Could not find the image
                    }
                }
                if (ImagePath32 != "")
                {
                    try
                    {
                        m_pbData.LargeImage = new BitmapImage(new Uri(ImagePath32));
                    }
                    catch
                    {
                        //Could not find the image
                    }
                }

                m_pbData.ToolTip = Tooltip;


                PushButton m_pb = Panel.AddItem(m_pbData) as PushButton;

                return true;
            }
            catch
            {
                return false;
            }
        }



    }
}
