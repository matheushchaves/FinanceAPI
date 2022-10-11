using AutoMapper;
using FinanceAPI.Data;
using FinanceAPI.Data.DTOs;
using FinanceAPI.Helpers.Http;
using FinanceAPI.Models;
using FinanceAPI.Models.DynamicTable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FinanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
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
        private Categoria get(Guid id, bool isTrack= false)
        {
            IQueryable<Categoria> query = asQueryable().Where(categoriaDb => categoriaDb.Id == id);
            if (isTrack)
                return query.FirstOrDefault();
            return query.AsNoTracking().FirstOrDefault();
        }

        private Usuario getUserAuth(string email)
        {
            return _uow.UsuarioRepository.AsQueryable().Where(usuarioDb => usuarioDb.Email == email).AsNoTracking().FirstOrDefault();
        }



        [EnableCors("DevPolicy")]
        [Authorize]
        [HttpPost("load-metadata")]
        public IActionResult PostMetadata()
        {
            return GetMetadata();
        }
        [EnableCors("DevPolicy")]
        [Authorize]
        [HttpGet("metadata")]
        public IActionResult GetMetadata()
        {

            Metadata m = new()
            {
                Title = "Categoria",
                Actions = new Actions()
                {
                    Save = "categoria",
                    SaveNew = "categoria/new",
                    Detail = "categoria/detail/:id",
                    Duplicate = "categoria/duplicate",
                    Edit = "categoria/edit/:id",
                    New = "categoria/new",
                    Remove = true,
                    RemoveAll = true
                },
                Fields = new List<Field>()
                {
                    new Field(){ Order= 1, Label= "Id", Property="id", Key=true, Disabled=true, Visible=false  },
                    new Field(){ Order= 2, Label= "Descrição", Property="descricao", Required=true },
                    new Field(){ Order= 3, Label= "Ícone", Type="icon", Property="icone", Required=true, Options= Icons.GetIcones()},
                    new Field(){ Order= 4, Label= "Dt. Criação", Property="datacriacao", Type="dateTime", Disabled = true },
                    new Field(){ Order= 5, Label= "Dt. Alteração", Property="dataalteracao", Type="dateTime", Disabled = true },
                    new Field(){ Order= 6, Label= "Bloqueado", Property="bloqueado", Type= "boolean" },
                }
            };


            return Ok(m);
        }

        [EnableCors("DevPolicy")]
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PageParams pageParams, string? descricao)
        {
            try
            {
                Usuario usuario = getUserAuth(User.Identity.Name);
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
                    if (usuario.Id != categoria.Id)
                        return BadRequest(new ApiReturn()
                        {
                            Code = "400",
                            Message = $"Categoria Id {categoria.Id} é de um usuário diferente do autenticado (Id: {usuario.Id}) !",
                        });                    
                    return Ok(_mapper.Map<CategoriaDTO>(categoria));
                }
                else
                {
                    IQueryable<Categoria> queryable = asQueryable();

                    queryable = queryable.Where(c => c.UsuarioId == usuario.Id);
                    
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

        [EnableCors("DevPolicy")]
        [Authorize]
        [HttpPost]
        public IActionResult Add(CategoriaDTO categoria)
        {
            try
            {
                Usuario usuario = getUserAuth(User.Identity.Name);
                Categoria categoriaDb = get(categoria.Id, true);
                if (categoriaDb is not null)
                    return BadRequest(new ApiReturn()
                    {
                        Code = "400",
                        Message = $"Já existe um registro com Id {categoria.Id}",
                    });
                categoriaDb = new Categoria();
                _mapper.Map<CategoriaDTO, Categoria>(categoria, categoriaDb);
                categoriaDb.Datacriacao = DateTimeOffset.Now;
                categoriaDb.Dataalteracao = DateTimeOffset.Now;
                categoriaDb.Usuario = usuario;
                categoriaDb.UsuarioId = usuario.Id;
                _uow.CategoriaRepository.Update(categoriaDb);
                _uow.Commit();
                return Ok(_mapper.Map<CategoriaDTO>(categoriaDb));
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
        [EnableCors("DevPolicy")]
        [Authorize]
        [HttpPut]
        public IActionResult Edit(CategoriaDTO categoriaDTO)
        {
            try
            {
                Usuario usuario = getUserAuth(User.Identity.Name);
                Categoria categoria = get(categoriaDTO.Id);
                if (categoria is null)
                    return BadRequest(new ApiReturn()
                    {
                        Code = "400",
                        Message = $"Não existe um registro com Id {categoriaDTO.Id}",
                    });
                if (categoria.UsuarioId != usuario.Id)
                    return BadRequest(new ApiReturn()
                    {
                        Code = "400",
                        Message = $"Categoria Id {categoriaDTO.Id} é de um usuário diferente do autenticado (Id: {usuario.Id}) !",
                    });
                _mapper.Map<CategoriaDTO, Categoria>(categoriaDTO, categoria);
                categoria.Dataalteracao = DateTimeOffset.Now;
                _uow.CategoriaRepository.Update(categoria);
                _uow.Commit();
                return Ok(_mapper.Map<CategoriaDTO>(categoria));
            }
            catch (Exception e)
            {
                return BadRequest(new ApiReturn()
                {
                    Code = "400",
                    Message = $"Falha ao alterar registro!",
                    Type = "error",
                    DetailedMessage = e.InnerException is null ? e.Message : e.InnerException.Message
                });
            }
        }

        [EnableCors("DevPolicy")]
        [Authorize]
        [HttpDelete]
        public IActionResult Delete(Guid categoriaId)
        {
            try
            {
                Usuario usuario = getUserAuth(User.Identity.Name);
                Categoria categoria = get(categoriaId);
                if (categoria is null)
                    return BadRequest(new ApiReturn()
                    {
                        Code = "400",
                        Message = $"Não existe um registro com Id {categoriaId}",
                    });
                if (categoria.UsuarioId != usuario.Id)
                    return BadRequest(new ApiReturn()
                    {
                        Code = "400",
                        Message = $"Categoria Id {categoriaId} é de um usuário diferente do autenticado (Id: {usuario.Id}) !",
                    });
                _uow.CategoriaRepository.Delete(categoria);
                _uow.Commit();
                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(new ApiReturn()
                {
                    Code = "400",
                    Message = $"Falha ao alterar registro!",
                    Type = "error",
                    DetailedMessage = e.InnerException is null ? e.Message : e.InnerException.Message
                });
            }
        }



    }
}
