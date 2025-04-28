using System.Text.Json.Serialization;

namespace E_CommerceAPI.ENTITES.DTOs
{
    public class ResponseDto    //providing one class for returned data (one type of returned data) --> helps in frontend
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public bool IsSucceeded { get; set; }
        public object? Model { get; set; }
        [JsonIgnore]
        public ICollection<object>? Models { get; set; }

    }
}
