using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class ModuleWeaver
{
    public ModuleDefinition ModuleDefinition { get; set; }

    public Action<string> LogInfo { get; set; }

    /// <summary>
    /// Will a list of all the msbuild constants. 
    /// A copy of the contents of the $(DefineConstants). OPTIONAL
    /// </summary>
    public List<string> DefineConstants { get; set; }

    /// <summary>
    /// Will contain the full directory path of the target project. 
    /// A copy of $(ProjectDir). OPTIONAL
    /// </summary>
    public string ProjectDirectoryPath { get; set; }

    /// <summary>
    /// Will contain the full path of the target assembly. OPTIONAL
    /// </summary>
    public string AssemblyFilePath { get; set; }

    public ModuleWeaver()
    {
        LogInfo = m => { };
    }

    public void Execute()
    {
        var typeSystem = ModuleDefinition.TypeSystem;
        var liveForms = ModuleDefinition.ReadModule(Path.Combine(ProjectDirectoryPath, "bin", "Debug", "Live.Forms.dll"));
        var extensionsType = liveForms.Types.First(t => t.FullName == "Live.Forms.Extensions");
        var watchMethod = ModuleDefinition.Import(extensionsType.Methods.First(m => m.Name == "Watch"));

        foreach (var type in ModuleDefinition.Types)
        {
            LogInfo("Interating over: " + type.FullName + ", " + ProjectDirectoryPath);

            if (Inherits(type, "Xamarin.Forms.Element"))
            {
                var ctor = type.Methods.FirstOrDefault(m => m.IsConstructor && m.IsPublic);
                var method = type.Methods.FirstOrDefault(m => m.Name == "InitializeComponent" && m.IsPrivate);
                if (ctor != null && method != null)
                {
                    LogInfo(type.Name + " has InitializeComponent!");

                    var doc = ctor.Body.Instructions[0].SequencePoint.Document;
                    string xamlFile = doc.Url.Substring(0, doc.Url.Length - 3);
                    LogInfo("Document found at: " + xamlFile);

                    var processor = method.Body.GetILProcessor();
                    processor.Remove(processor.Body.Instructions.Last());
                    processor.Emit(OpCodes.Ldarg_0);
                    processor.Emit(OpCodes.Ldstr, xamlFile);
                    processor.Emit(OpCodes.Call, watchMethod);
                    processor.Emit(OpCodes.Ret);
                }
            }
        }

        LogInfo("ModuleWeaver DONE!");
    }

    private bool Inherits(TypeDefinition type, string name)
    {
        while (type != null)
        {
            if (type.FullName == name)
                return true;

            type = type.BaseType?.Resolve();
        }
        return false;
    }
}