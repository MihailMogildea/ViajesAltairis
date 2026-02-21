using MediatR;
using ViajesAltairis.Application.Features.Admin.ReviewResponses.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ReviewResponses.Commands;

public record CreateReviewResponseCommand(long ReviewId, long UserId, string Comment) : IRequest<ReviewResponseDto>;

public class CreateReviewResponseHandler : IRequestHandler<CreateReviewResponseCommand, ReviewResponseDto>
{
    private readonly IRepository<ReviewResponse> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReviewResponseHandler(IRepository<ReviewResponse> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReviewResponseDto> Handle(CreateReviewResponseCommand request, CancellationToken cancellationToken)
    {
        var entity = new ReviewResponse
        {
            ReviewId = request.ReviewId,
            UserId = request.UserId,
            Comment = request.Comment
        };
        await _repository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ReviewResponseDto(entity.Id, entity.ReviewId, entity.UserId, "", entity.Comment, entity.CreatedAt);
    }
}
