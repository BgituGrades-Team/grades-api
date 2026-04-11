using AutoMapper;
using BgituGrades.Application.DTOs;
using BgituGrades.Application.Models.Transfer;
using BgituGrades.Domain.Entities;

namespace BgituGrades.Application.Mappings
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
