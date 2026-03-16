using AutoMapper;
using BgituGrades.DTO;
using BgituGrades.Entities;
using BgituGrades.Models.Transfer;
using BgituGrades.Repositories;

namespace BgituGrades.Services
{
    public interface ITransferService
    {
        Task<IEnumerable<TransferResponse>> GetAllTransfersAsync(CancellationToken cancellationToken);
        Task<TransferResponse> CreateTransferAsync(CreateTransferRequest request, CancellationToken cancellationToken);
        Task<TransferResponse?> GetTransferByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<TransferResponse>> GetTransfersByGroupAndDisciplineAsync(int groupId, int disciplineId, CancellationToken cancellationToken);
        Task<bool> UpdateTransferAsync(UpdateTransferRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteTransferAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<TransferDTO>> GetAllTransfersDtoAsync(CancellationToken cancellationToken);
        Task<TransferDTO?> GetTransferDtoByIdAsync(int id, CancellationToken cancellationToken);
    }
    public class TransferService(ITransferRepository transferRepository, IMapper mapper) : ITransferService
    {
        private readonly ITransferRepository _transferRepository = transferRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<TransferResponse> CreateTransferAsync(CreateTransferRequest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Transfer>(request);
            var createdEntity = await _transferRepository.CreateTransferAsync(entity, cancellationToken: cancellationToken);
            return _mapper.Map<TransferResponse>(createdEntity);
        }

        public async Task<bool> DeleteTransferAsync(int id, CancellationToken cancellationToken)
        {
            return await _transferRepository.DeleteTransferAsync(id, cancellationToken: cancellationToken);
        }

        public async Task<TransferResponse?> GetTransferByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _transferRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            return entity == null ? null : _mapper.Map<TransferResponse>(entity);
        }

        public async Task<IEnumerable<TransferResponse>> GetAllTransfersAsync(CancellationToken cancellationToken)
        {
            var entities = await _transferRepository.GetAllTransfersAsync(cancellationToken: cancellationToken);
            return _mapper.Map<IEnumerable<TransferResponse>>(entities);
        }

        public async Task<bool> UpdateTransferAsync(UpdateTransferRequest request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Transfer>(request);
            return await _transferRepository.UpdateTransferAsync(entity, cancellationToken: cancellationToken);
        }

        public async Task<IEnumerable<TransferDTO>> GetAllTransfersDtoAsync(CancellationToken cancellationToken)
        {
            var entities = await _transferRepository.GetAllTransfersAsync(cancellationToken: cancellationToken);
            return _mapper.Map<IEnumerable<TransferDTO>>(entities);
        }

        public async Task<TransferDTO?> GetTransferDtoByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _transferRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            return entity == null ? null : _mapper.Map<TransferDTO>(entity);
        }

        public async Task<IEnumerable<TransferResponse>> GetTransfersByGroupAndDisciplineAsync(int groupId, int disciplineId, CancellationToken cancellationToken)
        {
            var entities = await _transferRepository.GetTransfersByGroupAndDisciplineAsync(groupId, disciplineId, cancellationToken: cancellationToken);
            var response = _mapper.Map<IEnumerable<TransferResponse>>(entities);
            return response;
        }
    }
}
