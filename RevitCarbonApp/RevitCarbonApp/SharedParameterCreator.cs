using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;

namespace RevitCarbonApp
{
    internal class SharedParameterCreator
    {
        /// <summary>
        /// Check the carbon shared parameter exists
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static bool CarbonSharedParameterCheck(Document doc, string parameterName)
        {           
            Element collector = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfClass(typeof(SharedParameterElement))
                .Where(x => x.Name == parameterName).FirstOrDefault();
            
            if (collector != null)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        /// <summary>
        /// Add Shared Parameter to a project. See https://github.com/jeremytammik/RevitSdkSamples/blob/ab02648986189e3d488272fe9a98b291f84f56d1/SDK/Samples/DoorSwing/CS/DoorSharedParameters.cs
        /// </summary>
        /// <param name="uiapp"></param>
        /// <returns></returns>
        public static bool AddCarbonSharedParameter(UIApplication uiapp, string parameterName)
        {
            // Create a new Binding object with the categories to which the parameter will be bound.
            CategorySet categories = uiapp.Application.Create.NewCategorySet();

            // Get the required category and insert into the CategorySet. NOTE: This could be made user-defined
            Category wallCategory = uiapp.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory.OST_Walls);
            categories.Insert(wallCategory);
            Category floorCategory = uiapp.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory.OST_Floors);
            categories.Insert(floorCategory);
            Category strColumnsCategory = uiapp.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralColumns);
            categories.Insert(strColumnsCategory);
            Category strFramingCategory = uiapp.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFraming);
            categories.Insert(strFramingCategory);

            // Create type binding for Carbon Value parameter
            TypeBinding typeBinding = uiapp.Application.Create.NewTypeBinding(categories);
            BindingMap bindingMap = uiapp.ActiveUIDocument.Document.ParameterBindings;

            // Open the shared parameters file 
            // via the private method AccessOrCreateSharedParameterFile
            DefinitionFile defFile = HelpersParameter.AccessOrCreateSharedParameterFile(uiapp.Application);
            if (null == defFile)
            {
                return false;
            }

            // Access an existing or create a new group in the shared parameters file
            DefinitionGroups defGroups = defFile.Groups;
            string groupName = "ICE Carbon SharedParameters";
            DefinitionGroup defGroup = defGroups.get_Item(groupName);
            if (null == defGroup)
            {
                defGroup = defGroups.Create(groupName);
            }

            // Access an existing or create a new external parameter definition belongs to a specific group.

            foreach (Category category in categories)
            {

                if (!HelpersParameter.AlreadyAddedSharedParameter(uiapp.ActiveUIDocument.Document, parameterName, category))
                {
                    Definition carbonParameter = defGroup.Definitions.get_Item(parameterName);

                    if (null == carbonParameter)
                    {
                        ExternalDefinitionCreationOptions ExternalDefinitionCreationOptions1 = new ExternalDefinitionCreationOptions(parameterName, SpecTypeId.String.Text);
                        carbonParameter = defGroup.Definitions.Create(ExternalDefinitionCreationOptions1);
                    }

                    // Add the binding and definition to the document.
                    bindingMap.Insert(carbonParameter, typeBinding, BuiltInParameterGroup.PG_TEXT);
                }

            }
            



            return true;
        }
        }
}
