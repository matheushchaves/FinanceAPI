using AutoMapper;
using FinanceAPI.Data;
using FinanceAPI.Data.DTOs;
using FinanceAPI.Helpers.Http;
using FinanceAPI.Models;
using FinanceAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        /// <summary>
        /// Acesso a todos os repositorios
        /// </summary>
        /// <param name="uow"></param>
        public UsuariosController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        /// <summary>
        ///  Buscar o registro por id, usado em todos os endpoints
        ///  
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Usuario localizado no banco ou objeto padrao, sem tracking para persistir no banco</returns>
        private Usuario get(Guid id)
        {
            return asQueryable().Where(usuarioDb => usuarioDb.Id == id).AsNoTracking().FirstOrDefault();
        }
        /// <summary>
        /// Prove query para realizar buscas no banco.
        /// </summary>
        /// <returns></returns>
        private IQueryable<Usuario> asQueryable()
        {
            return _uow.UsuarioRepository.AsQueryable();
        }
        /// <summary>
        /// Endpoint para buscar usuario no banco:
        /// Se for passado um Id deve retornar um usuario unico;
        /// Se não é retornado uma lista do tipo PageList (modelo PO-UI)
        /// </summary>
        /// <param name="pageParams">Parametros padroes para paginaçao e demais entidades</param>
        /// <param name="nome">Paramero para filtrar por nome com paginacao</param>
        /// <param name="email">Parametro para filtrar por email com paginacao</param>
        /// <returns>Usuario, ApiReturn, ou PageList</returns>
        [HttpGet]
        [Authorize]
        [EnableCors("DevPolicy")]
        public async Task<IActionResult> Get([FromQuery] PageParams pageParams, string? nome, string? email)
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
                    Usuario usuario = get(id);
                    if (usuario == null)
                        return BadRequest(new ApiReturn()
                        {
                            Code = "400",
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
                    Code = "400",
                    Message = $"Falha ao incluir registro!",
                    Type = "error",
                    DetailedMessage =  e.InnerException is null ? e.Message : e.InnerException.Message
                });
            }
        }
        /// <summary>
        /// Adiciona usuario ao db
        /// </summary>
        /// <param name="usuario">Usuario usado para persistir no banco</param>
        /// <returns>Usuario persistido no banco ou ApiReturn se ocorrer erro</returns>
        [HttpPost]
        [AllowAnonymous]
        [EnableCors("DevPolicy")]
        public IActionResult Add(Usuario usuario)
        {
            
            try
            {
                bool existeEmail = asQueryable().Where<Usuario>(u => u.Email.ToLower().Equals(usuario.Email)).Count() > 0;
                if (existeEmail)
                    return BadRequest(new ApiReturn()
                    {
                        Code = "400",
                        Message = $"Já existe um usuário registrado com e-mail {usuario.Email}",                        
                    });
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
                    Code = "400",
                    Message = $"Falha ao incluir registro!",
                    Type = "error",
                    DetailedMessage =  e.InnerException is null ? e.Message : e.InnerException.Message
                });
            }
        }
        /// <summary>
        /// Edita usuario baseado no id
        /// </summary>
        /// <param name="usuario">Usuario usado para persistir no banco</param>
        /// <returns>Usuario persistido no banco ou ApiReturn se ocorrer erro</returns>
        [HttpPut]
        [Authorize]
        [EnableCors("DevPolicy")]
        public IActionResult Edit(Usuario usuario)
        {
            try
            {
                Usuario usuarioAtual = get(usuario.Id);
                if (usuarioAtual == null)
                    return BadRequest(new ApiReturn()
                    {
                        Code = "400",
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
                    Code = "400",
                    Message = $"Falha ao alterar registro!",
                    Type = "error",
                    DetailedMessage =  e.InnerException is null ? e.Message : e.InnerException.Message
                });
            }

        }

        /// <summary>
        /// Remove usuario baseado no id
        /// </summary>
        /// <param name="id">id do usuario</param>
        /// <returns>ApiReturn indicado o sucesso ou falha</returns>
        [HttpDelete]
        [Authorize]
        [EnableCors("DevPolicy")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                Usuario usuarioAtual = get(id);
                if (usuarioAtual == null)
                    return BadRequest(new ApiReturn()
                    {
                        Code = "400",
                        Message = $"Id {id} não existe!"
                    });

                _uow.UsuarioRepository.Delete(usuarioAtual);
                _uow.Commit();
                return Ok(new ApiReturn()
                    {
                        Code="204",
                        Message = $"Registro removido com sucesso!",
                        Type = "information",
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ApiReturn()
                {
                    Code = "400",
                    Message = $"Falha ao excluir registro!",
                    Type = "error",
                    DetailedMessage =  e.InnerException is null ? e.Message : e.InnerException.Message
                });
            }
            
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [EnableCors("DevPolicy")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] LoginDTO login)
        {
            // Recupera o usuário
            var user = asQueryable().Where<Usuario>(u => u.Email.ToLower().Equals(login.Email.ToLower()) && u.Senha.Equals(login.Senha)).AsNoTracking().SingleOrDefault();

            // Verifica se o usuário existe
            if (user == null)
                return NotFound(new ApiReturn() { Code = "400", Message= $"Usuário {login.Email} não encontrado!"});

            // Gera o Token
            var token = TokenService.GenerateToken(user);

            // Oculta a senha
            user.Senha = "";

            // Retorna os dados
            return Ok(new ApiReturn(){
                Code="201",
                Message = "Login realizado com sucesso!",
                DetailedMessage = token,
                Type = "information",
            });
        }

    }
}
