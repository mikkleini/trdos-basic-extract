using Kaitai;
using System.Text;
using System.IO;
using Microsoft.Extensions.FileSystemGlobbing;

namespace BasicDetokenizer;

class Program
{
    /// <summary>
    /// Main function
    /// </summary>
    /// <param name="args">Arguments</param>
    static int Main(String[] args)
    {
        Console.WriteLine("Sinclair kind-of Basic detokenizer");

        if (args.Length < 2)
        {
            Console.Error.WriteLine("Please provide two arguments:");
            Console.Error.WriteLine(" 1) Tokenized basic file(s) path (.B extension) or pattern.");
            Console.Error.WriteLine("    Can use pattern to include several files. E.g. C:\\Files\\*.B");
            Console.Error.WriteLine(" 2) Folder where to place detokenized file(s) with .bas extension.");
            return 1;
        }

        string inputPattern = args[0];
        string outputPath = args[1];
        string? inputFolder = Path.GetDirectoryName(inputPattern);

        // Verify arguments
        if ((inputFolder == null) || (!System.IO.Directory.Exists(inputFolder)))
        {
            Console.Error.WriteLine("Basic file(s) path/pattern does not exist.");
            return 2;
        }

        if (!System.IO.Directory.Exists(outputPath))
        {
            Console.Error.WriteLine("Extract folder does not exist.");
            return 3;
        }

        // Create files matcher
        Matcher matcher = new Matcher();
        matcher.AddInclude(inputPattern.Substring(inputFolder.Length));
        string[] inputFiles = matcher.GetResultsInFullPath(inputFolder).ToArray();
        if (inputFiles.Length == 0)
        {
            Console.WriteLine("No file(s) found");
            return 4;
        }

        // Detokenize files
        Detokenizer detok = new Detokenizer();

        foreach (string inputFile in inputFiles)
        {
            string fileNameOnly = Path.GetFileNameWithoutExtension(inputFile);
            string newFileName = Path.Combine(inputFolder, fileNameOnly + ".bas");

            // Try to detokenize
            Console.Write($"Detokenizing {fileNameOnly}... ".PadRight(26));
            byte[] data = File.ReadAllBytes(inputFile);
            bool result = detok.Detokenize(data, out string code);
            Console.WriteLine(result ? "OK" : "ERROR (See file)");

            // Try to save
            try
            {
                File.WriteAllText(newFileName, code);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
        }

        return 0;
    }
}