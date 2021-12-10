namespace EcuDox
{
    public class AvailableMap
    {
        public AvailableMap(string id, string displayName, bool active)
        {
            this.Id = id;
            this.DisplayName = displayName;
            this.Active = active;
        }

        public string Id;
        public string DisplayName;
        public bool Active;
    }
}
