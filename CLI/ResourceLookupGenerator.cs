using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CLI
{
    public class ResourceLookupGenerator
    {
        private readonly StringBuilder _builder = new StringBuilder();

        public void Generate(IEnumerable<string> resourceKeys, string filePath, string fullClassName)
        {
            ComposeContent(resourceKeys, fullClassName);
            File.WriteAllText(Path.Combine(filePath, GetFileNameFromNamespace(fullClassName).className + ".cs"), _builder.ToString());
        }

        private void ComposeContent(IEnumerable<string> resourceKeys, string fullClassName)
        {
            _builder.AppendLine("using System.Windows;");
            _builder.AppendLine();
            var nameInfo = GetFileNameFromNamespace(fullClassName);
            _builder.Append("namespace ");
            _builder.Append(nameInfo.namespaceName);
            _builder.AppendLine();

            OpenBody();
            AppendIndentation("//Generated file, any changes will be discarded, overwritten", 4);
            AppendIndentation($"public static class {nameInfo.className}", 4);
            OpenBody(4);

            foreach (var resourceKey in resourceKeys)
            {
                GenerateProperty(resourceKey);
            }

            CloseBody(4);
            CloseBody();
        }

        private void GenerateProperty(string resourceKey)
        {
            AppendIndentation($"public static object {resourceKey} => Application.Current?.Resources[\"{resourceKey}\"];", 8);
            _builder.AppendLine();
        }

        private void AppendIndentation(string text, int indentation)
        {
            _builder.Append(' ', indentation);
            _builder.Append(text);
            _builder.AppendLine();
        }

        private void OpenBody(int indentation = 0) => AppendIndentation("{", indentation);
        private void CloseBody(int indentation = 0) => AppendIndentation("}", indentation);

        public static (string className, string namespaceName) GetFileNameFromNamespace(string namespaceText)
        {
            var classNameParts = namespaceText.Split('.');
            var namespaceName = string.Join(".", classNameParts.Take(classNameParts.Count() - 1));
            return (classNameParts[classNameParts.Length - 1], namespaceName);
        }
    }
}
