using AutoMapper;
using Repository.UnitOfWork.Abstractions;
using Domain.Core.Aggreggates;
using MediatR;

namespace CrossCutting.CQRS.Queries;

public sealed class GetByIdQueryHandler<T> : IRequestHandler<GetByIdQuery<T>, T> where T : BaseDomain, new()
{
    private readonly IUnitOfWork<T> _unitOfWork;
    private readonly IMapper _mapper;

    public GetByIdQueryHandler(IUnitOfWork<T> unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<T> Handle(GetByIdQuery<T> request, CancellationToken cancellationToken)
    {
        var entityToFind = _mapper.Map<T>(request);
        return await _unitOfWork.Repository.Get(entityToFind.Id);
    }
}
