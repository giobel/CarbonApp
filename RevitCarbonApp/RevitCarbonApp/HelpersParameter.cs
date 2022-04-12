using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitCarbonApp
{
    class HelpersParameter
    {
        /// <summary>
        /// Access an existing or create a new shared parameters file.
        /// </summary>
        /// <param name="app">Revit Application.</param>
        /// <returns>the shared parameters file.</returns>
        public static DefinitionFile AccessOrCreateSharedParameterFile(Application app)
        {
            // The location of this command assembly
            string currentCommandAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            // The path of ourselves shared parameters file
            string sharedParameterFilePath = Path.GetDirectoryName(currentCommandAssemblyPath);
            sharedParameterFilePath = sharedParameterFilePath + "\\MySharedParameterFile.txt";

            //Method's return
            DefinitionFile sharedParameterFile = null;

            // Check if the file exits
            System.IO.FileInfo documentMessage = new FileInfo(sharedParameterFilePath);
            bool fileExist = documentMessage.Exists;

            // Create file for external shared parameter since it does not exist
            if (!fileExist)
            {
                FileStream fileFlow = File.Create(sharedParameterFilePath);
                fileFlow.Close();
            }

            // Set ourselves file to the externalSharedParameterFile 
            app.SharedParametersFilename = sharedParameterFilePath;
            sharedParameterFile = app.OpenSharedParameterFile();

            return sharedParameterFile;
        }

        /// <summary>
        /// Has the specific document shared parameter already been added ago?
        /// </summary>
        /// <param name="doc">Revit project in which the shared parameter will be added.</param>
        /// <param name="paraName">the name of the shared parameter.</param>
        /// <param name="boundCategory">Which category the parameter will bind to</param>
        /// <returns>Returns true if already added ago else returns false.</returns>
        public static bool AlreadyAddedSharedParameter(Document doc, string paraName, BuiltInCategory boundCategory)
        {
            try
            {
                BindingMap bindingMap = doc.ParameterBindings;
                DefinitionBindingMapIterator bindingMapIter = bindingMap.ForwardIterator();

                while (bindingMapIter.MoveNext())
                {
                    if (bindingMapIter.Key.Name.Equals(paraName))
                    {
                        ElementBinding binding = bindingMapIter.Current as ElementBinding;
                        CategorySet categories = binding.Categories;

                        foreach (Category category in categories)
                        {
                            if ((BuiltInCategory)category.Id.IntegerValue == boundCategory)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
        public static bool AlreadyAddedSharedParameter(Document doc, string paraName, Category boundCategory)
        {
            try
            {
                BindingMap bindingMap = doc.ParameterBindings;
                DefinitionBindingMapIterator bindingMapIter = bindingMap.ForwardIterator();

                while (bindingMapIter.MoveNext())
                {
                    if (bindingMapIter.Key.Name.Equals(paraName))
                    {
                        ElementBinding binding = bindingMapIter.Current as ElementBinding;
                        CategorySet categories = binding.Categories;

                        foreach (Category category in categories)
                        {
                            if (category == boundCategory)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
    }
}
