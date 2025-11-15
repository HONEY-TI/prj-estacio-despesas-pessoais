using Repository.UnitOfWork.Abstractions;
using Domain.Core.Aggreggates;
using MediatR;

namespace CrossCutting.CQRS.Queries;

public sealed class GetAllQueryHandler<T> : IRequestHandler<GetAllQuery<T>, IEnumerable<T>> where T : BaseDomain, new()
{
    private readonly IUnitOfWork<T> _unitOfWork;
    
    public GetAllQueryHandler(IUnitOfWork<T> unitOfWork)
    {
        _unitOfWork = unitOfWork;    
    }

    public async Task<IEnumerable<T>> Handle(GetAllQuery<T> request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Repository.GetAll();
    }
}
