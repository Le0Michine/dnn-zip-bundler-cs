namespace Zipper.Model
{
    public interface IEntry
    {
        string Name { get; set; }
        byte[] Bytes { get; set; }
    }
}