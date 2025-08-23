namespace CMD.SaveLoadSystem
{
    /// <summary>Классификация чанка (оставь/расширяй под себя).</summary>
    public enum ESaveType
    {
        component = 0, // Любой ISaveLoadObject не являющийся сущностью
        entity = 1, // BaseEntityRuntime снапшот
        custom = 2 // Что-то своё
    }
}
