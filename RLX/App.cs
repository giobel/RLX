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

                #region PIPES
                RibbonPanel pipes = GetSetRibbonPanel(a, tabName, "Pipes+Fittings+Hangers");

                if (AddPushButton(pipes, "btnCopyFittingsClosestParams", "Fitting Copy" + Environment.NewLine + "Closest Params", null, Resource1.copy, "RLX.PipeFittCopyClosestPipeParams", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button 1", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (AddPushButton(pipes, "btnCopyHangersClosestParams", "Hangers Copy" + Environment.NewLine + "Closest Params", null, Resource1.copy, "RLX.MechEquipCopyClosestPipeParams", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button 1", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (AddPushButton(pipes, "bnnFindChainagesPipes", "Find Pipes\nChainages", null, Resource1.chainages, "RLX.FindChainagePipes", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button 6", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (AddPushButton(pipes, "btnPipesTitle", "Fill Pipes\nTitle", null, Resource1.fill, "RLX.FillPipeTitles", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button 01", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (AddPushButton(pipes, "btnPipesFittTitle", "Fill Pipes\nFitting Title", null, Resource1.fill, "RLX.FillPipeFittingsTitle", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button 02", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (AddPushButton(pipes, "btnHangersTitle", "Fill Hangers\nTitle", null, Resource1.fill, "RLX.FillHangersTitles", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button 03", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (AddPushButton(pipes, "btnFillUniclass", "Fill Uniclass", null, Resource1.fill, "RLX.FillPipesAndHangersUniclassParams", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button FillPipesAndHangersUniclassParams", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (AddPushButton(pipes, "btnPipeMaterial", "Fill Pipes\nMaterial", null, Resource1.material, "RLX.FillPipesmaterials", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button Fill Pipes Material", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                #endregion

                #region IDS


                RibbonPanel UniqueIDS = GetSetRibbonPanel(a, tabName, "Fill Unique Identifiers");

                if (AddPushButton(UniqueIDS, "FillUID", "Fill RLX_ID", null, Resource1.fill, "RLX.FillUniqueId", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button 0", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


                if (AddPushButton(UniqueIDS, "btnCabinetIDs", "Cabinets IDs", null, Resource1.cabinet, "RLX.GenerateFurnitureIDS", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button GenerateFurnitureIDS", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (AddPushButton(UniqueIDS, "btnNozzles", "Nozzles IDs", null, Resource1.nozzles, "RLX.GenerateNozzlesIDS", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button GenerateFurnitureIDS", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


                if (AddPushButton(UniqueIDS, "btnSprinklers", "Sprinklers IDs", null, Resource1.sprinklers, "RLX.GenerateSprinklersIDS", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button GenerateSprinklersIDS", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (AddPushButton(UniqueIDS, "btnValves", "Valves IDs", null, Resource1.valves, "RLX.GenerateValvesIDS", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button GenerateValvesIDS", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                    #endregion

                    #region EXPLODE
                    RibbonPanel explodeFamilies = GetSetRibbonPanel(a, tabName, "Explode Families");

                if (AddPushButton(explodeFamilies, "familyInspector", "Inspector", null, Resource1.zoom, "RLX.FamilyInstanceInspector", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button FamilyInstanceInspector", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (AddPushButton(explodeFamilies, "hydrantExplode", "Hydrant", null, Resource1.hydrantExplode, "RLX.HydrantExplode", "Refer to documentation") == false)
                    {
                        MessageBox.Show("Failed to add button 1", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    #endregion

                    #region UTILITIES

                    RibbonPanel utilities = GetSetRibbonPanel(a, tabName, "Utilities");

                if (AddPushButton(utilities, "btnFillXYZ", "Fill XYZ", null, Resource1.fill, "RLX.FillXYZ", "Refer to documentation") == false)
                {
                    MessageBox.Show("Failed to add button Fill XYZ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


                if (AddPushButton(utilities, "CopyOldIds", "Copy Old Ids", null, Resource1.copy, "RLX.CopyOLDids", "Refer to documentation") == false)
                    {
                        MessageBox.Show("Failed to add button 2", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                    if (AddPushButton(utilities, "ColorByIDS", "Color" + Environment.NewLine + "By DS_ID", null, Resource1.colorBy, "RLX.ColorByDSId", "Refer to documentation") == false)
                    {
                        MessageBox.Show("Failed to add button 3", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (AddPushButton(utilities, "ColorByUID", "Color" + Environment.NewLine + "By RLX_UID", null, Resource1.colorBy, "RLX.ColorByRLXuid", "Refer to documentation") == false)
                    {
                        MessageBox.Show("Failed to add button 4", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                    if (AddPushButton(utilities, "FindChainages", "Find\nChainages", null, Resource1.chainages, "RLX.FindChainages", "Refer to documentation") == false)
                    {
                        MessageBox.Show("Failed to add button 5", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (AddPushButton(utilities, "CopyParams", "Copy\nParameters", null, Resource1.copy, "RLX.CopyParams", "Refer to documentation") == false)
                    {
                        MessageBox.Show("Failed to add button 7", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (AddPushButton(utilities, "CopyParamsMultiple", "Copy\nParameters+", null, Resource1.copy, "RLX.CopyParamsMultipleObjs", "Refer to documentation") == false)
                    {
                        MessageBox.Show("Failed to add button 8", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (AddPushButton(utilities, "btnFillCommonParams", "Fill Common\nParams", null, Resource1.fill, "RLX.FillCommonParams", "Refer to documentation") == false)
                    {
                        MessageBox.Show("Failed to add button 8", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (AddPushButton(utilities, "btnFillEmptyDescriptions", "Fill Empty\nDescriptions", null, Resource1.fill, "RLX.FillEmptyDescriptions", "Refer to documentation") == false)
                    {
                        MessageBox.Show("Failed to add button 8", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

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
