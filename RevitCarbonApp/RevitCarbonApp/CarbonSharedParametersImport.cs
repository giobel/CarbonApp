#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitCarbonApp.HelperClasses;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace RevitCarbonApp
{
    [Transaction(TransactionMode.Manual)]
    public class CarbonSharedParametersImport : IExternalCommand
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

            // Access current selection

            CreateSharedParametersBase csp = new CreateSharedParametersBase();
            csp.CreateSharedParameters(doc, app);

            return Result.Succeeded;
        }
    }
}
