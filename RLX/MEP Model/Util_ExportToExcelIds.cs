#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class Util_ExportToExcelIds : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            ViewScheduleExportOptions opt = new ViewScheduleExportOptions();

            opt.Title = false;
            opt.HeadersFootersBlanks = false;
            opt.FieldDelimiter = ";";

            List<string> paramsToExport = new List<string>(){
                "RLX_Type","RLX_UniqueIdentifier","RLX_Title","RLX_Description","RLX_MainMaterial",
                "RLX_ClassificationUniclassSs_Description","RLX_ClassificationUniclassSs_Number",
                "RLX_ClassificationUniclassEF_Description","RLX_ClassificationUniclassEF_Number",
                "RLX_ClassificationUniclassPr_Description","RLX_ClassificationUniclassPr_Number",
                "RLX_CoordinatesX","RLX_CoordinatesY","RLX_CoordinatesZ",
                "RLX_Facility","RLX_GridReferenceSystem","RLX_Location","RLX_MaintenanceCost",
                "RLX_Space","RLX_Specification","RLX_System","RLX_Zone","RLX_ActualCost",
                };

            string folderName = @"C:\Digital Sapiens\Digital Sapiens\Sergio Ros - MEP Buildings SilverTown and Greenwich\Z13 Silvertown Portal -  RLX Incoming information\06_Asset Tagging";

            IList<Element> elementsToExport = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(Helpers.RLXcatFilter()).WhereElementIsNotElementType().ToList();

            StringBuilder sb = new StringBuilder();



            var grouped = elementsToExport.GroupBy(x => x.LookupParameter("RLX_UniqueIdentifier").AsValueString());


            foreach (var group in grouped)
            {
                Helpers.DoElementsHaveSameParameterValues(group.ToList(), paramsToExport);
                sb.AppendLine(group.Key + "\t" + group.First().Id + "\t" + Helpers.DoElementsHaveSameParameterValues(group.ToList(), paramsToExport));
            }

            string outputFile = folderName + '\\' + "Z13_0001_CMD_All_Visible_Elements.csv";


            File.WriteAllText(outputFile, sb.ToString());

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = outputFile;
            process.Start();

            return Result.Succeeded;
        }
    }
}
