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
using System.Security.Cryptography;
using System.Text;
using RG = Rhino.Geometry;
#endregion

namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class CopyTitlesAndDescr : IExternalCommand
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

            //select all the pipes visible in view
            IList<Element> visiblePipes = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_PipeCurves)
                                                                                                .WhereElementIsNotElementType()
                                                                                                .ToElements();
            //select all fittings and hangers in view
            List<BuiltInCategory> builtInCats = new List<BuiltInCategory>();
            builtInCats.Add(BuiltInCategory.OST_PipeFitting);
            builtInCats.Add(BuiltInCategory.OST_MechanicalEquipment);

            ElementMulticategoryFilter filter1 = new ElementMulticategoryFilter(builtInCats);

            IList<Element> visibleFittingsAndHangers = new FilteredElementCollector(doc, doc.ActiveView.Id).WherePasses(filter1).WhereElementIsNotElementType().ToElements();


            //group by RLX_UniqueID. This returns a list of list of pipes
            // [0] [UID1pipe1, UID1pipe2, UID1pipe3...]
            // [1] [UID2pipe1, UID2pipe2, UID2pipe3...]
            //  ...
            var pipesGroupByUid = visiblePipes.GroupBy(x => x.LookupParameter("RLX_UniqueIdentifier"));

             
            //to modify the revit document we need to open and close a transaction
            using (Transaction t = new Transaction(doc, "Copy titles and descr"))
            {

            t.Start();


            //loop through each pipe 
            foreach (var pipes in pipesGroupByUid)
            {
                //get the uid of the first pipe
                string uid = pipes.First().LookupParameter("RLX_UniqueIdentifier").AsValueString();

                //get the title and description of the first pipe in the list
                string title = pipes.First().LookupParameter("RLX_Title").AsValueString();
                string description = pipes.First().LookupParameter("RLX_Description").AsValueString();

                //assign the title and description to all the other pipes that have the same uid
                foreach (var pipe in pipes)
                {
                    pipe.LookupParameter("RLX_Title").Set(title);
                    pipe.LookupParameter("RLX_Description").Set(description);
                }

                //find the corresponding hangers and fittings
                var correpsondingHF = visibleFittingsAndHangers.Where(x => x.LookupParameter("RLX_UniqueIdentifier").AsValueString() == uid);
                
                //loop through all corresponing hangers and fittings and set the title and description to match the pipe
                foreach (var hangerAndfitting in correpsondingHF)
                {
                    hangerAndfitting.LookupParameter("RLX_Title").Set(title);
                    hangerAndfitting.LookupParameter("RLX_Description").Set(description);

                }

            }
                //close the transaction
                t.Commit();

            }
            return Result.Succeeded;
            
        }
    }
}
