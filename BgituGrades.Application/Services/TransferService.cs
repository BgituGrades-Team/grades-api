using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Interfaces;
using BgituGrades.Application.Models.Transfer;
using BgituGrades.Domain.Entities;
using BgituGrades.Domain.Interfaces;

namespace BgituGrades.Application.Services
{
    
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

        public async Task<TransferResponse?> GetTransferByClassIdAndDateAsync(int classId, DateOnly originalDate, CancellationToken cancellationToken)
        {
            var entity = await _transferRepository.GetByClassIdAndDateAsync(classId, originalDate, cancellationToken: cancellationToken);
            return entity == null ? null : _mapper.Map<TransferResponse>(entity);
        }

        public async Task<List<TransferResponse>> GetAllTransfersAsync(CancellationToken cancellationToken)
        {
            var entities = await _transferRepository.GetAllTransfersAsync(cancellationToken: cancellationToken);
            return _mapper.Map<List<TransferResponse>>(entities);
        }

        public async Task<bool> UpdateTransferAsync(UpdateTransferRequest request, CancellationToken cancellationToken)
        {
            var entity = await _transferRepository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);
            if (entity == null)
                return false;

            entity.NewDate = request.NewDate;
            return await _transferRepository.UpdateTransferAsync(entity, cancellationToken: cancellationToken);
        }

        public async Task<List<TransferDTO>> GetAllTransfersDtoAsync(CancellationToken cancellationToken)
        {
            var entities = await _transferRepository.GetAllTransfersAsync(cancellationToken: cancellationToken);
            return _mapper.Map<List<TransferDTO>>(entities);
        }

        public async Task<TransferDTO?> GetTransferDtoByIdAsync(int id, CancellationToken cancellationToken)
        {
            var entity = await _transferRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            return entity == null ? null : _mapper.Map<TransferDTO>(entity);
        }

        public async Task<List<TransferResponse>> GetTransfersByGroupAndDisciplineAsync(int groupId, int disciplineId, CancellationToken cancellationToken)
        {
            var entities = await _transferRepository.GetTransfersByGroupAndDisciplineAsync(groupId, disciplineId, cancellationToken: cancellationToken);
            var response = _mapper.Map<List<TransferResponse>>(entities);
            return response;
        }
    }
}
