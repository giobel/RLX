#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
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


                    #region UTILITIES

                RibbonPanel utilities = GetSetRibbonPanel(a, tabName, "Utilities");



                    if (AddPushButton(utilities, "btnZoom", "Zoom\nSelected", null, Resource1.zoom, "RLX.ZoomSelected", "Refer to documentation") == false)
                    {
                        MessageBox.Show("Failed to add button 9", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    #endregion
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

        private Boolean AddPushButton(RibbonPanel Panel, string ButtonName, string ButtonText, System.Drawing.Bitmap Image16, System.Drawing.Bitmap Image32, string dllClass, string Tooltip)
        {

            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            try
            {
                PushButtonData m_pbData = new PushButtonData(ButtonName, ButtonText, thisAssemblyPath, dllClass);

                if (Image16 != null)
                {
                    try
                    {
                        m_pbData.Image = Convert(Image16);
                    }
                    catch
                    {
                        //Could not find the image
                    }
                }
                if (Image32 != null)
                {
                    try
                    {
                        m_pbData.LargeImage = Convert(Image32);
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

        public BitmapImage Convert(System.Drawing.Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

    }
}
