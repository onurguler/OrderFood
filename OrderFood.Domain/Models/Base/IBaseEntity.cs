namespace OrderFood.Domain.Models.Base
{
    public interface IBaseEntity<TId>
    {
        TId Id { get; set; }
    }
}
