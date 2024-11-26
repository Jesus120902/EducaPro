using Microsoft.ML.Data;

namespace EducaPro.Models
{
    public class ChatInput
    {
        public string Mensaje { get; set; }
    }

    public class ChatPrediction
    {
        [ColumnName("PredictedLabel")]
        public string Categoria { get; set; }

        public float[] Score { get; set; }
    }

    public class ChatData
{
    [LoadColumn(0)]
    public string Mensaje { get; set; }

    [LoadColumn(1)]
    [ColumnName("Label")] // Mapear "Categoria" a "Label"
    public string Categoria { get; set; }
}

}