using AutoMapper;
using FinanceAPI.Data;
using FinanceAPI.Helpers.Http;
using FinanceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public UsuariosController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        private Usuario get(Guid id)
        {
            return asQueryable().Where(usuarioDb => usuarioDb.Id == id).AsNoTracking().FirstOrDefault();
        }
        private IQueryable<Usuario> asQueryable()
        {
            return _uow.UsuarioRepository.AsQueryable();
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PageParams pageParams, string? nome, string? email)
        {
            try
            {
                if (pageParams is null)
                    return BadRequest(new ApiReturn()
                    {
                        Sucess = false,
                        Message = $"Parâmetros não existem!"
                    });
                if (pageParams.Id != null)
                {
                    Guid id = (Guid)pageParams.Id;
                    Usuario usuario = get(id);
                    if (usuario == null)
                        return BadRequest(new ApiReturn()
                        {
                            Sucess = false,
                            Message = $"Id {id} não existe!"
                        });
                    return Ok(usuario);
                }
                else
                {
                    IQueryable<Usuario> queryable = asQueryable();
                    switch (pageParams.Orderby.ToLower())
                    {
                        case "datacriacao": queryable = queryable.OrderBy(u => u.Datacriacao); break;
                        case "dataalteracao": queryable = queryable.OrderBy(u => u.Dataalteracao); break;
                        case "id": queryable = queryable.OrderBy(u => u.Id); break;
                        case "nome": queryable = queryable.OrderBy(u => u.Nome); break;
                        case "email": queryable = queryable.OrderBy(u => u.Email); break;
                    }


                    if (pageParams.Filter.Length > 0)
                        queryable = queryable
                            .Where(u => 
                                    u.Nome.ToLower().Contains(pageParams.Filter.ToLower()) || 
                                    u.Email.ToLower().Contains(pageParams.Filter.ToLower()
                                    ));    
                    
                    if (pageParams.Bloqueado != null)
                        queryable = queryable.Where(u => u.Bloqueado == pageParams.Bloqueado);
                    if (nome != null)
                        queryable = queryable.Where(u => u.Nome.ToLower().Contains(nome.ToLower()));
                    if (email != null)
                        queryable = queryable.Where(u => u.Email.ToLower().Contains(email.ToLower()));

                    return Ok(await PageList<Usuario>.ToPagedList(queryable, pageParams.Page, pageParams.PageSize));
                }
            }
            catch (Exception e)
            {
                return BadRequest(new ApiReturn()
                {
                    Sucess = false,
                    Message = $"Falha ao incluir registro!",
                    Detail = e.Message
                });
            }
        }

        [HttpPost]
        public IActionResult Add(Usuario usuario)
        {
            try
            {
                usuario.Datacriacao = DateTimeOffset.Now;
                usuario.Dataalteracao = DateTimeOffset.Now;
                _uow.UsuarioRepository.Insert(usuario);
                _uow.Commit();
                return Ok(usuario);
            }
            catch (Exception e)
            {
                return BadRequest(new ApiReturn()
                {
                    Sucess = false,
                    Message = $"Falha ao incluir registro!",
                    Detail = e.Message
                });
            }
        }
        [HttpPut]
        public IActionResult Edit(Usuario usuario)
        {
            try
            {
                Usuario usuarioAtual = get(usuario.Id);
                if (usuarioAtual == null)
                    return BadRequest(new ApiReturn()
                    {
                        Sucess = false,
                        Message = $"Id {usuario.Id} não existe!"
                    });

                usuario.Dataalteracao = DateTimeOffset.Now;
                usuario.Datacriacao = usuarioAtual.Datacriacao;
                _uow.UsuarioRepository.Update(usuario);
                _uow.Commit();
                return Ok(usuario);
            }
            catch (Exception e)
            {
                return BadRequest(new ApiReturn()
                {
                    Sucess = false,
                    Message = $"Falha ao alterar registro!",
                    Detail = e.Message
                });
            }

        }

        [HttpDelete]
        public IActionResult Delete(Guid id)
        {
            try
            {
                Usuario usuarioAtual = get(id);
                if (usuarioAtual == null)
                    return BadRequest(new ApiReturn()
                    {
                        Sucess = false,
                        Message = $"Id {id} não existe!"
                    });

                _uow.UsuarioRepository.Delete(usuarioAtual);
                _uow.Commit();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new ApiReturn()
                {
                    Sucess = false,
                    Message = $"Falha ao excluir registro!",
                    Detail = e.Message
                });
            }

        }

    }
}
