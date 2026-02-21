using MediatR;
using ViajesAltairis.Application.Features.Admin.ReviewResponses.Dtos;
using ViajesAltairis.Domain.Entities;
using ViajesAltairis.Domain.Interfaces;

namespace ViajesAltairis.Application.Features.Admin.ReviewResponses.Commands;

public record UpdateReviewResponseCommand(long Id, string Comment) : IRequest<ReviewResponseDto>;

public class UpdateReviewResponseHandler : IRequestHandler<UpdateReviewResponseCommand, ReviewResponseDto>
{
    private readonly IRepository<ReviewResponse> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReviewResponseHandler(IRepository<ReviewResponse> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReviewResponseDto> Handle(UpdateReviewResponseCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"ReviewResponse {request.Id} not found.");
        entity.Comment = request.Comment;
        await _repository.UpdateAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new ReviewResponseDto(entity.Id, entity.ReviewId, entity.UserId, "", entity.Comment, entity.CreatedAt);
    }
}
