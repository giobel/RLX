
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;


namespace RLX
{
    [Transaction(TransactionMode.Manual)]
    public class DeleteProjParametrs : IExternalCommand
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




            BindingMap map = uidoc.Application.ActiveUIDocument.Document.ParameterBindings;

            DefinitionBindingMapIterator it = map.ForwardIterator();

            it.Reset();

            List<Definition> defs = new List<Definition>();




            using (Transaction t = new Transaction(doc, "Delete Parameters"))
            {

                t.Start();





                while (it.MoveNext())

                {

                    if (it.Key != null && it.Key.Name.Contains("System_"))// && type.Equals(it.Key.ParameterType))

                    {
                        defs.Add(it.Key);
                    }

                }

                int counter = 0;

                foreach (Definition def in defs)
                {

                if (def != null)
                {
                    map.Remove(def);
                        counter++;
                }

                }

                t.Commit();

                TaskDialog.Show("R", counter.ToString());
            }


            return Result.Succeeded;

        }
    }
}
