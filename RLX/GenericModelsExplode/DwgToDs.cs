#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class DwgToDs : IExternalCommand
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

            Reference importRef = uidoc.Selection.PickObject(ObjectType.Element, "Pick family");

            Options opt = new Options();

            opt.DetailLevel = ViewDetailLevel.Fine;

            Element importElement = doc.GetElement(importRef);

            GeometryElement geometryElement = importElement.get_Geometry(new Options());

            using (Transaction t = new Transaction(doc, "Import dwg"))
            {
                t.Start();

                if (geometryElement != null)
                {
                    // Iterate through geometry objects
                    foreach (GeometryObject geometryObject in geometryElement)
                    {
                        if (geometryObject is GeometryInstance geometryInstance)
                        {
                            // Handle geometry instance
                            GeometryElement instanceGeometry = geometryInstance.GetInstanceGeometry();

                            //var gStyle = doc.GetElement(item.GraphicsStyleId) as GraphicsStyle;

                            //string layer = gStyle.GraphicsStyleCategory.Name;

                            foreach (GeometryObject geom in instanceGeometry)
                            {
                                if (geom is Solid)
                                {
                                    Solid solidGeo = geom as Solid;

                                    IList<Solid> solids = SolidUtils.SplitVolumes(solidGeo);

                                    foreach (var s in solids)
                                    {
                                        FaceArray faces = s.Faces;

                                        
                                        var gStyle = doc.GetElement(faces.get_Item(0).GraphicsStyleId) as GraphicsStyle;
                                        string layer = gStyle.GraphicsStyleCategory.Name;

                                        DirectShape dsd = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));
                                        dsd.SetShape(new List<GeometryObject>() { s });
                                        dsd.LookupParameter("Comments").Set(layer);

                                    }
                                }
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
