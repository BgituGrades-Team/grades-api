using AutoMapper;
using BgituGrades.DTO;
using BgituGrades.Entities;
using BgituGrades.Models.Transfer;

namespace BgituGrades.Mapping
{
    public class TransferProfile : Profile
    {
        public TransferProfile()
        {
            CreateMap<CreateTransferRequest, Transfer>();
            CreateMap<UpdateTransferRequest, Transfer>();
            CreateMap<Transfer, TransferResponse>();
            CreateMap<Transfer, TransferDTO>();
            CreateMap<TransferDTO, Transfer>();
        }
    }
}
