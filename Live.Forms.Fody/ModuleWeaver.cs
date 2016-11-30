using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public class ModuleWeaver
{
    public ModuleDefinition ModuleDefinition { get; set; }

    public Action<string> LogInfo { get; set; }

    /// <summary>
    /// NOTE: need to check DEBUG and skip the weaver otherwise
    /// </summary>
    public List<string> DefineConstants { get; set; }

    /// <summary>
    /// NOTE: could to use this for main path of app
    /// </summary>
    public string ProjectDirectoryPath { get; set; }

    public ModuleWeaver()
    {
        LogInfo = m => { };
    }

    public void Execute()
    {
        var typeSystem = ModuleDefinition.TypeSystem;
        var liveForms = ModuleDefinition.ReadModule(Path.Combine("bin", "Release", "Live.Forms.dll"));
        var extensionsType = liveForms.Types.First(t => t.FullName == "Live.Forms.Extensions");
        var watchMethod = ModuleDefinition.Import(extensionsType.Methods.First(m => m.Name == "Watch"));

        foreach (var type in ModuleDefinition.Types)
        {
            LogInfo("Interating over: " + type.FullName + ", " + ProjectDirectoryPath);

            if (Inherits(type, "Xamarin.Forms.Element"))
            {
                var method = type.Methods.FirstOrDefault(m => m.Name == "InitializeComponent" && m.IsPrivate);
                if (method != null)
                {
                    LogInfo(type.Name + " has InitializeComponent!");

                    var processor = method.Body.GetILProcessor();
                    processor.Remove(processor.Body.Instructions.Last());
                    processor.Emit(OpCodes.Ldobj, type);
                    processor.Emit(OpCodes.Call, watchMethod);
                    processor.Emit(OpCodes.Ret);
                }
            }
        }

        LogInfo("WatchWeaver DONE!");
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