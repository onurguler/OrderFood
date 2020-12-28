namespace OrderFood.Domain.Base
{
    public interface IBaseEntity<TId>
    {
        TId Id { get; set; }
    }
}
