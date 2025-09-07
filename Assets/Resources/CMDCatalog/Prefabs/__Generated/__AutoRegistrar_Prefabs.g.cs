// AUTO-GENERATED. DO NOT EDIT
#pragma warning disable
using CMD.Services;
using UnityEngine.Scripting;

[Preserve]
public sealed class __AutoRegistrar_Prefabs : ICatalogRegistrar
{
    public void RegisterInto(ICatalog c)
    {
        var folder = new ResourcesFolderCatalog<global::UnityEngine.GameObject>("CMDCatalog/Prefabs");
        c.RegisterFolder(folder);
    }
}
