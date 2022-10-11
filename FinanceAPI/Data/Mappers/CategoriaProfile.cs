using AutoMapper;
using FinanceAPI.Data.DTOs;
using FinanceAPI.Models;

namespace FinanceAPI.Data.Mappers
{
    public class CategoriaProfile: Profile
    {
        public CategoriaProfile()
        {
            CreateMap<CategoriaDTO, Categoria>();
            CreateMap<Categoria, CategoriaDTO>();
        }
    }
}
