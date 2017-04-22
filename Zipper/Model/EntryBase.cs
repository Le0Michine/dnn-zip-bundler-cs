namespace Zipper.Model
{
    public class EntryBase : IEntry
    {
        public string Name { get; set; }
        public byte[] Bytes { get; set; }
    }
}