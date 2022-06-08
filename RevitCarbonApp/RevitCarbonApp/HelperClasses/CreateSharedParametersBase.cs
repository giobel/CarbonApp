using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.IO;


namespace RevitCarbonApp.HelperClasses
{
    internal class CreateSharedParametersBase
    {

        public void CreateSharedParameters(Document doc, Application app)
        {
            //built-in categories
            Category walls = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Walls);
            Category structFraming = doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFraming);
            Category structColumns = doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralColumns);
            Category structFdn = doc.Settings.Categories.get_Item(BuiltInCategory.OST_StructuralFoundation);
            Category stairs = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Stairs);

            //collection of categories
            CategorySet categorySet = app.Create.NewCategorySet();
            categorySet.Insert(walls);
            categorySet.Insert(structFraming);
            categorySet.Insert(structColumns);
            categorySet.Insert(structFdn);
            categorySet.Insert(stairs);

            string originalFile = app.SharedParametersFilename;

            //possibly look to file this in the static resources of the solution
            string tempfile = @"C:\Users\j_was\Documents\SharedParameters.txt";

            try
            {
                //checking if file exists on the system
                if (File.Exists(tempfile))
                {
                    //switches the default location to the temp one
                    app.SharedParametersFilename = tempfile;

                    //Opens the shared parameter file
                    DefinitionFile sharedParameter = app.OpenSharedParameterFile();

                    //looping through the definition groups in the shared parameters file
                    foreach (DefinitionGroup dg in sharedParameter.Groups)
                    {
                        if (dg.Name == "CARBON APP")
                        {
                            ExternalDefinition externalDefinition = dg.Definitions.get_Item("Carbon Coefficient") as ExternalDefinition;

                            using (Transaction t = new Transaction(doc))
                            {
                                t.Start("Adding Shared Parameters");
                                //parameter binding
                                InstanceBinding newIB = app.Create.NewInstanceBinding(categorySet);
                                //parameter group to text - *we might want to add it to a different section*
                                doc.ParameterBindings.Insert(externalDefinition, newIB, BuiltInParameterGroup.PG_TITLE);
                                t.Commit();
                            }

                        }

                    }


                }
                else
                {
                    // just a warning - *Potentially show windows dialog to load the file manually if missing
                    TaskDialog.Show("Shared Parameters Needed", "Please located shared parameters file and place in appdata");
                }

            }
            catch(Exception e)
            {
                
                TaskDialog.Show("error", "An error has occured"+Environment.NewLine+(e.Message));
            }
            finally
            {
                //reset to original file

                app.SharedParametersFilename = originalFile;

            }

        }
    }
}
