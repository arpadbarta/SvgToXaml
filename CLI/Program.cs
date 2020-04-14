using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SvgConverter;

namespace CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Count() < 2)
                throw new ArgumentException("Please makes sure that you provide a source folder and a target file path");

            var workingDirectory = args[0];
            var filePath = args[1];
            var fullClassName = args[2];

            var svgFiles =  ConverterLogic.SvgFilesFromFolder(workingDirectory,true);

            var content = new StringBuilder();

            var resourceNames = new List<string>();

            string GetName(string baseName)
            {
                var nameParts = baseName.Split('_');
                var resourceNameBuilder = new StringBuilder();

                foreach (var namePart in nameParts)
                {
                    var local = namePart.Replace("-", string.Empty);

                    if (int.TryParse(local, out int _))
                    {
                        local = "N" + local;
                    }

                    resourceNameBuilder.Append(ToUpper(local));
                }

                var resourcesName = resourceNameBuilder.ToString();

                Console.WriteLine();
                Console.WriteLine($"Generated resource name: {resourcesName}");

                resourceNames.Add(resourcesName);

                return resourcesName;
            }

            foreach (var svgFile in svgFiles)
            {
                var resKey = new ResKeyInfo
                {
                    XamlName = Path.GetFileNameWithoutExtension(svgFile),
                    NameConverter = GetName
                };

                var result = ConverterLogic.SvgFileToXaml(svgFile, ResultMode.DrawingImage, resKey, false);

                content.Append(result);
            }

            var nameInfo = ResourceLookupGenerator.GetFileNameFromNamespace(fullClassName);

            Directory.CreateDirectory(filePath);

            File.WriteAllText(Path.Combine(filePath, nameInfo.className + ".xaml"), content.ToString());
            var resourceLookup = new ResourceLookupGenerator();
            resourceLookup.Generate(resourceNames, filePath, fullClassName);
        }

        private static string ToUpper(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
