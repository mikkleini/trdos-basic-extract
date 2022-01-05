using Kaitai;
using System.Text;
using System.IO;

namespace TRDOSExtract;

class Program
{
    /// <summary>
    /// Main function
    /// </summary>
    /// <param name="args">Arguments</param>
    static int Main(String[] args)
    {
        int result = 0;
        TrDosImage disk;

        Console.WriteLine("TR-DOS image extractor");

        if (args.Length < 2)
        {
            Console.Error.WriteLine("Please provide two arguments:");
            Console.Error.WriteLine(" 1) TR-DOS raw image file path.");
            Console.Error.WriteLine(" 2) Folder where to extract files.");
            return 1;
        }

        string inputFile = args[0];
        string outputPath = args[1];

        // Verify arguments
        if (!System.IO.File.Exists(inputFile))
        {
            Console.Error.WriteLine("TR-DOS raw image file not found.");
            return 2;
        }

        if (!System.IO.Directory.Exists(outputPath))
        {
            Console.Error.WriteLine("Extract folder does not exist.");
            return 2;
        }

        // Try to load
        Console.WriteLine($"Loading {inputFile}...");
        try
        {
            disk = TrDosImage.FromFile(inputFile);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 4;
        }

        // Show info
        Console.WriteLine("Volume:");
        Console.WriteLine($"  Name:      {disk.VolumeInfo.LabelASCII}");
        Console.WriteLine($"  Disk type: {disk.VolumeInfo.DiskType}");
        Console.WriteLine($"  Sides:     {disk.VolumeInfo.NumSides}");
        Console.WriteLine($"  Tracks:    {disk.VolumeInfo.NumTracks}");        
        Console.WriteLine($"  Files:     {disk.VolumeInfo.NumFiles}");
        Console.WriteLine($"  Deleted:   {disk.VolumeInfo.NumDeletedFiles}");
        Console.WriteLine();

        // Show files
        Console.WriteLine("Files:");
        Console.WriteLine("  Filename  Type Track  Sectors     Type specific           DELeted/TERMinator Saved as");
        Console.WriteLine("  ------------------------------------------------------------------------------------------------");

        foreach (Kaitai.File file in disk.Files)
        {
            string originalName = file.RawNameASCII;
            string osFriendlyFileName = OSFriendlyFileName(originalName) + $".{(char)file.Extension}";

            string info =
                $"  {originalName,-8}  {(char)file.Extension}    " +
                $"{file.StartingTrack,-5}  {file.StartingSector,-3} to {(file.StartingSector + file.LengthSectors - 1),-3}  ";

            int realLength;

            switch (file.PositionAndLength)
            {
                case PositionAndLengthBasic bl:
                    realLength = bl.ProgramLength;
                    info += $"Code:   {bl.ProgramLength,5}, Code+Data: {bl.ProgramAndDataLength,5}";
                    break;

                case PositionAndLengthCode cl:
                    realLength = cl.Length;
                    info += $"Start:  {cl.StartAddress,5}, Length:    {cl.Length,5}";
                    break;

                case PositionAndLengthGeneric gl:
                    realLength = gl.Length;
                    info += $"Length: {gl.Length,5}, Reserved:    {gl.Reserved,5}";
                    break;

                case PositionAndLengthPrint pl:
                    realLength = pl.Length;
                    info += $"Length: {pl.Length,5}, Reserved:    {pl.Reserved,5}";
                    break;

                default:
                    // Kaitia issue ?
                    realLength = 0;
                    break;
            }

            info += $"  {(file.IsDeleted ? "DEL" : "   ")} {(file.IsTerminator ? "TERM" : "    ")}  ";

            Console.Write(info);
            
            // Try to save to disk
            try
            {
                string path = Path.Combine(outputPath, osFriendlyFileName);
                System.IO.File.WriteAllBytes(path, file.Contents.Take(realLength).ToArray());
                Console.WriteLine(osFriendlyFileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving {osFriendlyFileName}");
                Console.Error.WriteLine(ex.Message);
                result = 4;
            }
        }

        return result;
    }

    /// <summary>
    /// Get OS friendly file name
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private static string OSFriendlyFileName(string fileName)
    {
        return new String(fileName.Select(c => (Path.GetInvalidFileNameChars().Contains(c) ? '_' : c)).ToArray());
    }
}