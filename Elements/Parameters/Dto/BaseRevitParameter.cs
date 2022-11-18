namespace ASRR.Revit.Core.Elements.Parameters.Dto
{
    public class BaseRevitParameter<T>
    {
        public string name { get; set; }
        public T value { get; set; }
    }
}
