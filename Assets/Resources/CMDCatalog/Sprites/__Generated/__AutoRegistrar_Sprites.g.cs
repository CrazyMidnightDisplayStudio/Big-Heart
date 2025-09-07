// AUTO-GENERATED. DO NOT EDIT
#pragma warning disable
using CMD.Services;
using UnityEngine.Scripting;

[Preserve]
public sealed class __AutoRegistrar_Sprites : ICatalogRegistrar
{
    public void RegisterInto(ICatalog c)
    {
        var folder = new ResourcesFolderCatalog<global::UnityEngine.Sprite>("CMDCatalog/Sprites");
        c.RegisterFolder(folder);
    }
}
