using CopperGameTools.Builder;
using System.Diagnostics;

namespace CopperGameTools.CLI
{
    class Program
    {
        public static void Main(String[] args)
        {
            Console.WriteLine("Please make sure to keep CGT updated to ensure it works with newer CopperCube Engine Versions.\n" +
                "At the time of this build, version 6.6 is the newest one available.");

            // No subcommand used / no args?
            if (Utils.IsEmpty(args))
            {
                Console.WriteLine($"CopperGameTools v{Utils.GetVersion()} on {Utils.GetBuildDate()}\n" +
                    "No subcommand used.\n");
                Console.ReadKey();
                return;
            }

            switch (args[0])
            {
                case "pack":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("pack <project file>");
                        return;
                    }
                    try
                    {
                        if (!Directory.Exists("Publish"))
                            Directory.CreateDirectory("./Publish");

                        ProjectFile projFile = new ProjectFile(new FileInfo(args[1]));

                        string platform = projFile.GetKey("project.platform");
                        if (platform == "")
                        {
                            Console.WriteLine("No platform specified!");
                            return;
                        }

                        string projectFileName = projFile.GetKey("project.file");
                        if (projectFileName == "" || !File.Exists(projectFileName))
                        {
                            Console.WriteLine("CopperCube-Project file not found!");
                            return;
                        }

                        string executableFileName = Path.GetFileNameWithoutExtension(projectFileName);
                        executableFileName += ".exe";

                        string packPath = "Publish/" + platform + "/";

                        if (Directory.Exists(packPath))
                        {
                            Directory.Delete(packPath);
                        }

                        Directory.CreateDirectory(packPath);

                        Console.WriteLine("Copy " + "./Project/" + executableFileName + " to " + packPath);
                        File.Copy("./Project/" + executableFileName, packPath + executableFileName);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to load file!");
                    }
                    break;
                case "build+":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("build+ <project file>");
                        return;
                    }
                    try
                    {
                        ProjectBuilder builder = new ProjectBuilder(new ProjectFile(new FileInfo(args[1])));
                        ProjFileCheckResult projFileCheckResult = builder.ProjFile.CheckProjectFile();
                        switch (builder.Build().ResultType)
                        {
                            case ProjBuilderResultType.DoneNoErrors:
                                Console.WriteLine("Error: Not caused by project-file!");
                                break;
                            case ProjBuilderResultType.FailedNoErrors:
                                Console.WriteLine("Failed: Not caused by project-file!");
                                break;
                            case ProjBuilderResultType.FailedWithErrors:
                                Console.WriteLine("Failed: Caused by project-file!");
                                Utils.PrintErrors(projFileCheckResult);
                                break;
                        }

                        string projectFileName = builder.ProjFile.GetKey("project.file");
                        if (projectFileName == "" || !File.Exists(projectFileName))
                        {
                            Console.WriteLine("CopperCube-Project file not found!");
                            return;
                        }

                        string projectPlatform = builder.ProjFile.GetKey("project.platform");
                        if (projectPlatform == "")
                        {
                            Console.WriteLine("No platform specified!");
                            return;
                        }

                        Console.WriteLine("Creating Game Executable from Project " + projectFileName + " for " + projectPlatform);

                        Process editor = new Process();

                        editor.StartInfo.FileName = "coppercube.exe";
                        editor.StartInfo.Arguments = $"{new FileInfo(projectFileName).FullName} -publish:{projectPlatform} -quit";
                        editor.StartInfo.CreateNoWindow = false;

                        editor.Start();
                        editor.WaitForExit();
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to load file!");
                    }
                    break;
                case "build":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("build <project file>");
                        return;
                    }
                    try
                    {
                        ProjectBuilder builder = new ProjectBuilder(new ProjectFile(new FileInfo(args[1])));
                        ProjFileCheckResult projFileCheckResult = builder.ProjFile.CheckProjectFile();
                        switch (builder.Build().ResultType)
                        {
                            case ProjBuilderResultType.DoneNoErrors:
                                Console.WriteLine("No errors found.");
                                break;
                            case ProjBuilderResultType.FailedNoErrors:
                                Console.WriteLine("Failed with no errors.");
                                break;
                            case ProjBuilderResultType.FailedWithErrors:
                                Console.WriteLine("Failed with errors");
                                Utils.PrintErrors(projFileCheckResult);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to load file!");
                    }
                    break;

                case "check":
                    if (args.Length < 2)
                    {
                        Console.WriteLine("check <project file>");
                        return;
                    }
                    try
                    {
                        ProjFileCheckResult checkRes = new ProjectBuilder(new ProjectFile(new FileInfo(args[1]))).ProjFile.CheckProjectFile();
                        Utils.PrintErrors(checkRes);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Failed to load file!");
                    }
                    break;
            }
        }
    }

}
