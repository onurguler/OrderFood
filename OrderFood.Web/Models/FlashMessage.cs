namespace OrderFood.Web.Models
{
    public enum FlashMessageType
    {
        Success,
        Warning,
        Danger,
        Info
    }

    public class FlashMessage
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }
}
