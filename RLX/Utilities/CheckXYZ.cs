#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using RG = Rhino.Geometry;
#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class CheckXYZ : IExternalCommand
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

            double scale = 304.8;

            Element familySymbol = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel)
                .WhereElementIsElementType().Where(x => x.Name == "sphere").First();

            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_PipeCurves);
            builtInCats.Add(BuiltInCategory.OST_PipeAccessory);
            builtInCats.Add(BuiltInCategory.OST_Sprinklers);
            builtInCats.Add(BuiltInCategory.OST_GenericModel);
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);
            builtInCats.Add(BuiltInCategory.OST_Furniture);
            builtInCats.Add(BuiltInCategory.OST_PipeFitting);


            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);

            IList<Element> visibleElements = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();

            var grouped = visibleElements.GroupBy(x => x.LookupParameter("RLX_UniqueIdentifier").AsValueString());
            //FamilySymbol fs = new FilteredElementCollector(doc).OfClass(typeof(Family)).Where(x => x.Name == "sphere").Cast<FamilySymbol>().First();

            FamilySymbol fs = new FilteredElementCollector(doc) .OfClass(typeof(FamilySymbol))
        .First(q => q.Name.Equals("sphere"))
          as FamilySymbol;


            Transform surveyTransf = doc.ActiveProjectLocation.GetTotalTransform();

            ProjectLocation pl = doc.ActiveProjectLocation;
            Transform ttr = pl.GetTotalTransform().Inverse;

            XYZ pbp = BasePoint.GetProjectBasePoint(doc).SharedPosition;
            XYZ sp = BasePoint.GetSurveyPoint(doc).Position;

            // Calculate the offset from project base point to survey point
            XYZ offset = sp - pbp;

            using (Transaction t = new Transaction(doc, "Check xyz"))
            {
                t.Start();

                foreach (var visibleElementList in grouped) {

                    Element visibleElement = visibleElementList.First();

                    string xPt = visibleElement.LookupParameter("RLX_CoordinatesX").AsString();
                    string yPt = visibleElement.LookupParameter("RLX_CoordinatesY").AsString();
                    string zPt = visibleElement.LookupParameter("RLX_CoordinatesZ").AsString();

                    

                    XYZ ptEN = new XYZ(double.Parse(xPt), double.Parse(yPt), double.Parse(zPt));


                    XYZ pt = ptEN * 1000 / scale - pbp;

                    FamilyInstance fa = doc.Create.NewFamilyInstance(pt, fs, StructuralType.NonStructural);

                    fa.LookupParameter("RLX_CoordinatesX").Set(xPt);
                    fa.LookupParameter("RLX_CoordinatesY").Set(yPt);
                    fa.LookupParameter("RLX_CoordinatesZ").Set(zPt);


                    XYZ bendPnt = pt.Add(new XYZ (0, 1, 4));

                    XYZ endPnt = pt.Add(new XYZ(0, 2, 4));

                    Reference re = fa.GetReferences(FamilyInstanceReferenceType.CenterFrontBack).First();
                    
                    
                   // doc.Create.NewSpotCoordinate(doc.ActiveView, re, pt, bendPnt, endPnt, pt, false);
                
                
                }
                t.Commit();
            }

            return Result.Succeeded;
            
        }
    }
}
