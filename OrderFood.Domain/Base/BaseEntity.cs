namespace OrderFood.Domain.Base
{
    public class BaseEntity : IBaseEntity<long>
    {
        public long Id { get; set; }
    }
}
