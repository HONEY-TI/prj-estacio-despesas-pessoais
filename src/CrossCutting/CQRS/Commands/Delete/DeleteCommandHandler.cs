using AutoMapper;
using Repository.UnitOfWork.Abstractions;
using Domain.Core.Aggreggates;
using MediatR;

namespace CrossCutting.CQRS.Commands;

public sealed class DeleteCommandHandler<T> : IRequestHandler<DeleteCommand<T>, T> where T : BaseDomain, new()
{
    private readonly IUnitOfWork<T> _unitOfWork;
    private readonly IMapper _mapper;

    public DeleteCommandHandler(IUnitOfWork<T> unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<T> Handle(DeleteCommand<T> request, CancellationToken cancellationToken)
    {
        var entityToDelete = _mapper.Map<T>(request);

        var existingEntity = await _unitOfWork.Repository.Get(entityToDelete.Id);
        if (existingEntity is null)
            throw new InvalidOperationException($"{nameof(existingEntity)} not found !");

        await _unitOfWork.Repository.Delete(entityToDelete.Id);
        await _unitOfWork.CommitAsync();
        return entityToDelete;
    }
}
