#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace RevitCarbonApp
{
    [Transaction(TransactionMode.Manual)]
    public class SharedParameterCreatorCommand : IExternalCommand
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

            string paramName = "ICE Carbon Value";

            
            //Checks the model to see if the carbon coefficient parameter exists in the model, if not creates one.
            if (SharedParameterCreator.CarbonSharedParameterCheck(doc, paramName))
            {
                TaskDialog.Show("Result", "Parameter exists");
            }
            else
            {
                TaskDialog td = new TaskDialog("Result");
                td.MainContent = "Carbon shared parameter does not exists. \nWould you like to create it?";
                td.AddCommandLink(TaskDialogCommandLinkId.CommandLink1,"Create Parameter");
                td.AddCommandLink(TaskDialogCommandLinkId.CommandLink2,"Cancel");

                switch (td.Show())
                {
                    case TaskDialogResult.CommandLink1:
                        using (Transaction t = new Transaction(doc, "Add carbon parameter"))
                        {
                            t.Start();
                            SharedParameterCreator.AddCarbonSharedParameter(uiapp, paramName);
                            t.Commit();
                        }
                        break;

                    case TaskDialogResult.CommandLink2:                       
                        break;
                }

            }

            //datagrid UI which can quickly assign the carbon coefficient to elements. Possibly filter out elements by Workset, level, category and family type etc.

            //Checks that all the elements have values assigned to new shared parameter

            //If not ,a new UI object is displayed showing the unpopulated elements, if all elements aren’t null export to a CSV.

            return Result.Succeeded;
        }
    }
}
