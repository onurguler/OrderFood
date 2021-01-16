namespace OrderFood.Domain.Models.Base
{
    public class BaseEntity : IBaseEntity<long>
    {
        public long Id { get; set; }
    }
}
