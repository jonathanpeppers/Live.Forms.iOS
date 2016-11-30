using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public class ModuleWeaver
{
    public Action<string> LogInfo { get; set; }

    public List<string> DefineConstants { get; set; }

    public ModuleDefinition ModuleDefinition { get; set; }

    public ModuleWeaver()
    {
        LogInfo = m => { };
    }

    public void Execute()
    {
        foreach (var type in ModuleDefinition.Types)
        {
            LogInfo("Interating over: " + type.Name);

            if (type.HasMethods && type.Methods.Any(m => m.Name == "InitializeComponent"))
            {
                LogInfo(type.Name + " has InitializeComponent!");
            }
        }

        LogInfo("WatchWeaver DONE!");
    }
}