namespace TheSSS.DicomViewer.Common.Models.Imaging
{
    public class LookupTableOutput
    {
        public byte[] LutData { get; set; }
        public int NumberOfEntries { get; set; }
        public short FirstValueMapped { get; set; }
        public int BitsPerEntry { get; set; }
    }
}