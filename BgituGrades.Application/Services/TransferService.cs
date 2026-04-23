using AutoMapper;
using BgituGrades.Application.Caching;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Transfer;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;

namespace BgituGrades.Application.Services
{
    
    public class TransferService(ITransferRepository transferRepository, IMapper mapper, ICacheService cacheService) : ITransferService
    {
        private readonly ITransferRepository _transferRepository = transferRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ICacheService _cacheService = cacheService;

        public async Task<TransferResponse> CreateTransferAsync(CreateTransferRequest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Transfer>(request);
            var createdEntity = await _transferRepository.CreateTransferAsync(entity, cancellationToken: cancellationToken);
            await _cacheService.RemoveByTagAsync(CacheTags.Class(), cancellationToken);
            return _mapper.Map<TransferResponse>(createdEntity);
        }

        public async Task<bool> DeleteTransferAsync(int id, CancellationToken cancellationToken)
        {
            var success = await _transferRepository.DeleteTransferAsync(id, cancellationToken: cancellationToken);
            if (success)
                await _cacheService.RemoveByTagAsync(CacheTags.Class(), cancellationToken);
            return success;
         }

        public async Task<TransferResponse?> GetTransferByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _transferRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            return entity == null ? null : _mapper.Map<TransferResponse>(entity);
        }

        public async Task<TransferResponse?> GetTransferByClassIdAndDateAsync(int classId, DateOnly date, CancellationToken cancellationToken)
        {
            var entity = await _transferRepository.GetByClassIdAndDateAsync(classId, date, cancellationToken: cancellationToken);
            return entity == null ? null : _mapper.Map<TransferResponse>(entity);
        }

        public async Task<bool> UpdateTransferAsync(UpdateTransferRequest request, CancellationToken cancellationToken)
        {
            var entity = await _transferRepository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
            if (entity == null)
                return false;
            if (entity.OriginalDate == request.NewDate)
            {
                await _transferRepository.DeleteTransferAsync(entity.Id, cancellationToken: cancellationToken);
                await _cacheService.RemoveByTagAsync(CacheTags.Class(), cancellationToken);
                return true;
            }

            entity.NewDate = request.NewDate;
            await _cacheService.RemoveByTagAsync(CacheTags.Class(), cancellationToken);
            return await _transferRepository.UpdateTransferAsync(entity, cancellationToken: cancellationToken);
        }

        public async Task<List<TransferResponse>> GetTransfersByGroupAndDisciplineAsync(int groupId, int disciplineId, CancellationToken cancellationToken)
        {
            var entities = await _transferRepository.GetTransfersByGroupAndDisciplineAsync(groupId, disciplineId, cancellationToken: cancellationToken);
            var response = _mapper.Map<List<TransferResponse>>(entities);
            return response;
        }
    }
}
