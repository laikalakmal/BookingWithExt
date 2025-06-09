using MediatR;

public class DeleteProductCommand : IRequest<bool>
{
    public Guid Id { get; }

    public DeleteProductCommand(Guid id)
    {
        Id = id;
    }
}