// AUTO-GENERATED. DO NOT EDIT
#pragma warning disable
using CMD.Services;
using UnityEngine.Scripting;

[Preserve]
public sealed class __AutoRegistrar_ItemDefinitions : ICatalogRegistrar
{
    public void RegisterInto(ICatalog c)
    {
        var folder = new ResourcesFolderCatalog<global::BigHeart.ItemDefinition>("CMDCatalog/ItemDefinitions");
        c.RegisterFolder(folder);
    }
}
