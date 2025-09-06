// AUTO-GENERATED. DO NOT EDIT
#pragma warning disable
using CMD.Services;
using UnityEngine.Scripting;

[Preserve]
public sealed class __AutoRegistrar_CMD_Catalog_Prefabs : ICatalogRegistrar
{
    public void RegisterInto(ICatalog c)
    {
        var folder = new ResourcesFolderCatalog<global::UnityEngine.GameObject>("CMD.Catalog/Prefabs");
        c.RegisterFolder(folder);
    }
}
