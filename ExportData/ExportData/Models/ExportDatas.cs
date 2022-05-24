namespace ExportData.Models
{
    public class ExportDatas
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public int RelationId { get; set; }
        public int ExportDataTypeId { get; set; }
        public ExportDataType ExportDataType { get; set; }
        
        
    }
}
