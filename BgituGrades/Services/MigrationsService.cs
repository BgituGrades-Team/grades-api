using BgituGrades.Repositories;
using BgituGrades.Models.Mark;

namespace BgituGrades.Services
{
    public interface IMigrationService
    {
        Task DeleteAll(CancellationToken cancellationToken);
    }
    public class MigrationsService(IClassRepository classRepository, IDisciplineRepository disciplineRepository,
        IGroupRepository groupRepository, IMarkRepository markRepository, 
        IPresenceRepository presenceRepository, ITransferRepository transferRepository, IWorkRepository workRepository) : IMigrationService
    {
        private readonly IClassRepository _classRepository = classRepository;
        private readonly IDisciplineRepository _disciplineRepository = disciplineRepository;
        private readonly IGroupRepository _groupRepository = groupRepository;
        private readonly IPresenceRepository _presenceRepository = presenceRepository;
        private readonly ITransferRepository _transferRepository = transferRepository;
        private readonly IWorkRepository _workRepository = workRepository;
        private readonly IMarkRepository _markRepository = markRepository;
        public async Task DeleteAll(CancellationToken cancellationToken)
        {
            await _markRepository.DeleteAllAsync(cancellationToken: cancellationToken);
            await _classRepository.DeleteAllAsync(cancellationToken: cancellationToken);
            await _disciplineRepository.DeleteAllAsync(cancellationToken: cancellationToken);
            await _groupRepository.DeleteAllAsync(cancellationToken: cancellationToken);
            await _presenceRepository.DeleteAllAsync(cancellationToken: cancellationToken);
            await _transferRepository.DeleteAllAsync(cancellationToken: cancellationToken);
            await _workRepository.DeleteAllAsync(cancellationToken: cancellationToken);
        }
    }
}
