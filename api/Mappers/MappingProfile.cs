using AutoMapper;
using api.DTOs.Stock;
using api.DTOs.Comment;
using api.Models;

namespace api.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Stock Mappings
            CreateMap<Stock, StockDTO>();
            CreateMap<CreateStockRequestDTO, Stock>();
            CreateMap<UpdateStockRequestDTO, Stock>();

            // Comment Mappings
            CreateMap<Comment, CommentDTO>();
            CreateMap<CreateCommentRequestDTO, Comment>();
            CreateMap<UpdateCommentRequestDTO, Comment>();
        }
    }
}
