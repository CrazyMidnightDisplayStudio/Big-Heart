namespace CMD.Services
{
    public readonly struct CatalogKey<T> where T : UnityEngine.Object
    {
        public readonly string Value;
        public CatalogKey(string value) => Value = value;
        public override string ToString() => Value;
        public static implicit operator string(CatalogKey<T> k) => k.Value;
    }
}
