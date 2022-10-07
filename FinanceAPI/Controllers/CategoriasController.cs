using AutoMapper;
using FinanceAPI.Data;
using FinanceAPI.Data.DTOs;
using FinanceAPI.Helpers.Http;
using FinanceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FinanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController: ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        /// <summary>
        /// Acesso a todos os repositorios
        /// </summary>
        /// <param name="uow"></param>
        public CategoriasController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        /// <summary>
        /// Prove query para realizar buscas no banco.
        /// </summary>
        /// <returns></returns>
        private IQueryable<Categoria> asQueryable()
        {
            return _uow.CategoriaRepository.AsQueryable();
        }

        /// <summary>
        ///  Buscar o registro por id, usado em todos os endpoints
        ///  
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Usuario localizado no banco ou objeto padrao, sem tracking para persistir no banco</returns>
        private Categoria get(Guid id)
        {
            return asQueryable().Where(categoriaDb => categoriaDb.Id == id).AsNoTracking().FirstOrDefault();
        }

        private Usuario getUserAuth(string email)
        {
            return _uow.UsuarioRepository.AsQueryable().Where(usuarioDb => usuarioDb.Email == email).AsNoTracking().FirstOrDefault();
        }
        [HttpGet]
        [Authorize]
        [EnableCors("DevPolicy")]
        public async Task<IActionResult> Get([FromQuery] PageParams pageParams, string? descricao)
        {
            try
            {
                if (pageParams is null)
                    return BadRequest(new ApiReturn()
                    {
                        Code = "400",
                        Message = $"Parâmetros não existem!"
                    });
                if (pageParams.Id != null)
                {
                    Guid id = (Guid)pageParams.Id;
                    Categoria categoria = get(id);
                    if (categoria == null)
                        return BadRequest(new ApiReturn()
                        {
                            Code = "400",
                            Message = $"Id {id} não existe!"
                        });
                    return Ok(categoria);
                }
                else
                {
                    IQueryable<Categoria> queryable = asQueryable();
                    switch (pageParams.Orderby.ToLower())
                    {
                        case "datacriacao": queryable = queryable.OrderBy(c => c.Datacriacao); break;
                        case "dataalteracao": queryable = queryable.OrderBy(c => c.Dataalteracao); break;
                        case "id": queryable = queryable.OrderBy(c => c.Id); break;
                        case "descricao": queryable = queryable.OrderBy(c => c.Descricao); break;
                    }

                    if (pageParams.Filter.Length > 0)
                        queryable = queryable
                            .Where(c =>
                                    c.Descricao.ToLower().Contains(pageParams.Filter.ToLower()) 
                                    );

                    if (pageParams.Bloqueado != null)
                        queryable = queryable.Where(c => c.Bloqueado == pageParams.Bloqueado);
                    if (descricao != null)
                        queryable = queryable.Where(c => c.Descricao.ToLower().Contains(descricao.ToLower()));

                    return Ok(await PageList<Categoria, CategoriaDTO>.ToPagedList(queryable, pageParams.Page, pageParams.PageSize, _mapper));
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ApiReturn()
                {
                    Code = "400",
                    Message = $"Falha ao incluir registro!",
                    Type = "error",
                    DetailedMessage = e.InnerException is null ? e.Message : e.InnerException.Message
                });
            }
        }
        
        [HttpPost]
        [Authorize]
        [EnableCors("DevPolicy")]
        public IActionResult Add(Categoria categoria)
        {            
            Usuario usuario = getUserAuth(User.Identity.Name);
            try
            {
                Categoria categoriaDb = get(categoria.Id);
                if (categoriaDb is not null)
                    return BadRequest(new ApiReturn()
                    {
                        Code = "400",
                        Message = $"Já existe um registro com Id {categoria.Id}",
                    });
                categoria.Datacriacao = DateTimeOffset.Now;
                categoria.Dataalteracao = DateTimeOffset.Now;
                categoria.Usuario = usuario;
                categoria.UsuarioId = usuario.Id;
                _uow.CategoriaRepository.Update(categoria);
                _uow.Commit();
                return Ok(_mapper.Map<CategoriaDTO>(categoria));
            }
            catch (Exception e)
            {
                return BadRequest(new ApiReturn()
                {
                    Code = "400",
                    Message = $"Falha ao incluir registro!",
                    Type = "error",
                    DetailedMessage = e.InnerException is null ? e.Message : e.InnerException.Message
                });
            }
        }



    }
}
