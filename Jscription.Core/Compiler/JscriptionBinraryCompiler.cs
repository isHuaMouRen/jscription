using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Jscription.Core.Compiler
{
    public class JscriptionBinaryCompiler
    {
        public bool CompileToFile(string csharpCode, string outputExePath, out List<string> errors)
        {
            errors = new List<string>();

            var syntaxTree = CSharpSyntaxTree.ParseText(csharpCode);

            var references = new List<MetadataReference>();

            var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
                .Split(Path.PathSeparator);

            string[] neededAssemblies = { "System.Runtime", "System.Console", "System.IO", "System.Collections", "System.Text.RegularExpressions" };

            foreach (var path in trustedAssembliesPaths)
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                if (neededAssemblies.Contains(fileName, StringComparer.OrdinalIgnoreCase) || fileName == "mscorlib")
                {
                    references.Add(MetadataReference.CreateFromFile(path));
                }
            }

            var compilationOptions = new CSharpCompilationOptions(OutputKind.ConsoleApplication)
                .WithOptimizationLevel(OptimizationLevel.Release)
                .WithPlatform(Platform.AnyCpu);

            var compilation = CSharpCompilation.Create(
                assemblyName: Path.GetFileNameWithoutExtension(outputExePath),
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: compilationOptions
            );

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    foreach (var diagnostic in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
                    {
                        errors.Add($"[{diagnostic.Id}] Line {diagnostic.Location.GetLineSpan().StartLinePosition.Line + 1}: {diagnostic.GetMessage()}");
                    }
                    return false;
                }

                ms.Seek(0, SeekOrigin.Begin);
                using (var fileStream = File.Create(outputExePath))
                {
                    ms.CopyTo(fileStream);
                }
                return true;
            }
        }
    }
}