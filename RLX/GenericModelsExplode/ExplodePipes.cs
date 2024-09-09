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
    public class ExplodePipes : IExternalCommand
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

            Reference familyRef = uidoc.Selection.PickObject(ObjectType.Element, "Pick family");

            Options opt = new Options();

            opt.DetailLevel = ViewDetailLevel.Fine;


            Element familyInstance = doc.GetElement(familyRef) as Element;

           

            using (Transaction t = new Transaction(doc, "Explode family"))
            {


                t.Start();


            GeometryElement geometryElement = familyInstance.get_Geometry(opt);


                List<GeometryObject> geoObjects = new List<GeometryObject>();

                List<int> ids = new List<int>() {
                    12917,
12909,
12845,
8743,
8663,
8632

};

                List<GeometryObject> goes = new List<GeometryObject>();
                List<GeometryObject> elseObjs = new List<GeometryObject>();

                // Iterate over the geometry and add it to the list
                foreach (GeometryObject geoObj in geometryElement)
                {

                    //DirectShape directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));
                    //directShape.LookupParameter("Comments").Set(geoObj.Id.ToString());
                    //directShape.SetShape(new List<GeometryObject>(){ geoObj});


                    if (ids.Contains(geoObj.Id))
                    {
                        goes.Add(geoObj);
                    }
                    else
                    {
                        elseObjs.Add(geoObj);
                    }
                }





                try
                {
                    DirectShape directShape = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));

                    directShape.SetShape(goes);

                    DirectShape directShape1 = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_PipeCurves));

                    directShape1.SetShape(elseObjs);


                }
                catch { }
            






            t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
