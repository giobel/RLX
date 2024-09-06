#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using RG = Rhino.Geometry;

#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class ProcessDWG : IExternalCommand
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


            Reference alignmentsRef = uidoc.Selection.PickObject(ObjectType.Element, "Pick DWG");

            Element alignment = doc.GetElement(alignmentsRef);

            Options opts = new Options();

            GeometryElement alignmentGeometry = alignment.get_Geometry(opts);

            Element level = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Levels).First();


            using (Transaction t = new Transaction(doc, "Import alignments"))
            {
                t.Start();

                    SketchPlane sp = SketchPlane.Create(doc, level.Id);
        
                


            foreach (GeometryObject geoObj in alignmentGeometry)
            {

                GeometryInstance gi = geoObj as GeometryInstance;

                GeometryElement ge = gi.GetInstanceGeometry();

                foreach (var item in ge)
                {
                    var gStyle = doc.GetElement(item.GraphicsStyleId) as GraphicsStyle;

                    string layer = gStyle.GraphicsStyleCategory.Name;
                    

                    if (layer == "HE-Zz_35_20-M_RoadCentreline")
                    {
                        
                        string geoType = item.GetType().Name;
                        
                        if (geoType == "NurbSpline")
                        {
                            var curve = item as NurbSpline;

                            doc.Create.NewModelCurve(curve, sp);

                        }
                        
                    }

                }

            }
                t.Commit();

            }
            return Result.Succeeded;
        
}
    }
}
