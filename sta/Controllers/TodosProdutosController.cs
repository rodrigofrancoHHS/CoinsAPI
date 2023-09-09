using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sta.Models;
using Newtonsoft.Json;
using System.IO;
using PetaPoco;
using System.Data;
using MySql.Data.MySqlClient;
using System.Xml.Linq;

namespace sta.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodosProdutosController : ControllerBase
    {
        private readonly ProdutosContext _context;

        public TodosProdutosController(ProdutosContext context)
        {
            _context = context;
        }


        string connectionString = "Server=mysql-coinsproject.alwaysdata.net;Port=3306;Database=coinsproject_ricardo;Uid=326408_ricardo;Pwd=Rjgamer31;";


        // GET: api/TodoItems
        [HttpGet("ListadeProdutos")]
        public async Task<ActionResult<IEnumerable<TodosProdutosDTO>>> GetTodoItems()
        {
            using (var db = new Database(connectionString, "MySql.Data.MySqlClient")) 
            {
                var todosProdutos = await db.FetchAsync<TodosProdutos>("SELECT * FROM products");

                var responseItems = todosProdutos.Select(p => new TodosProdutosDTO
                {
                    Id = p.Id,
                    name = p.name,
                    desc = p.desc,
                    price = p.price,
                    rrp = p.rrp,
                    quantity = p.quantity,
                    img = p.img,
                    type = p.type
                }).ToList();

                return Ok(responseItems);
            }
        }

        // GET: api/TodoItems/5
        // <snippet_GetByID>
        [HttpGet("Apenas{id}")]
        public async Task<ActionResult<TodosProdutosDTO>> GetTodoProdutos(long id)
        {
            using (var db = new Database(connectionString, "MySql.Data.MySqlClient")) 
            {
                var todosProdutos = await db.SingleOrDefaultAsync<TodosProdutos>("SELECT * FROM products WHERE Id = @0", id);

                if (todosProdutos == null)
                {
                    return NotFound();
                }

                return ProdutosToDTO(todosProdutos);
            }
        }




        // INPUT: É O QUE ENVIA PARA DENTRO, OUTPUT: É O QUE RECEBE PARA FORA.


        [HttpGet("ListadeSweats")]
        public async Task<ActionResult<IEnumerable<TodosProdutosDTO>>> GetTodoItemsSweats()
        {
            using (var db = new Database(connectionString, "MySql.Data.MySqlClient")) 
            {
                var todosProdutos = await db.FetchAsync<TodosProdutos>("SELECT * FROM products WHERE type = 'sweat';");

                var responseItems = todosProdutos.Select(p => new TodosProdutosDTO
                {
                    Id = p.Id,
                    name = p.name,
                    desc = p.desc,
                    price = p.price,
                    rrp = p.rrp,
                    quantity = p.quantity,
                    img = p.img,
                    type = p.type
                }).ToList();

                return Ok(responseItems);
            }
        }








        [HttpGet("ListadeTshirts")]
        public async Task<ActionResult<IEnumerable<TodosProdutosDTO>>> GetTodoItemsTshirts()
        {
            using (var db = new Database(connectionString, "MySql.Data.MySqlClient")) 
            {
                var todosProdutos = await db.FetchAsync<TodosProdutos>("SELECT * FROM products WHERE type = 'tshirt';");

                var responseItems = todosProdutos.Select(p => new TodosProdutosDTO
                {
                    Id = p.Id,
                    name = p.name,
                    desc = p.desc,
                    price = p.price,
                    rrp = p.rrp,
                    quantity = p.quantity,
                    img = p.img,
                    type = p.type
                }).ToList();

                return Ok(responseItems);
            }
        }









        [HttpGet("ListadeCaps")]
        public async Task<ActionResult<IEnumerable<TodosProdutosDTO>>> GetTodoItemsCaps()
        {
            using (var db = new Database(connectionString, "MySql.Data.MySqlClient")) 
            {
                var todosProdutos = await db.FetchAsync<TodosProdutos>("SELECT * FROM products WHERE type = 'cap';");

                var responseItems = todosProdutos.Select(p => new TodosProdutosDTO
                {
                    Id = p.Id,
                    name = p.name,
                    desc = p.desc,
                    price = p.price,
                    rrp = p.rrp,
                    quantity = p.quantity,
                    img = p.img,
                    type = p.type
                }).ToList();

                return Ok(responseItems);
            }
        }












        [HttpPost("InserirAtualizarProdutos")]

        public async Task<ActionResult> PostTodosProdutos([FromBody] List<TodosProdutosDTO> todosProdutosDTOList)
        {
            using (var db = new Database(connectionString, "MySql.Data.MySqlClient"))
            {
                foreach (var todosProdutosDTO in todosProdutosDTOList)
                {
                    var produtoExistente = await db.SingleOrDefaultAsync<TodosProdutos>("SELECT * FROM products WHERE id = @0", todosProdutosDTO.Id);

                    if (produtoExistente == null)
                    {
                        // O produto não existe na base de dados, então vamos adicioná-lo
                        var novoProduto = new TodosProdutos
                        {
                            Id = todosProdutosDTO.Id,
                            name = todosProdutosDTO.name,
                            desc = todosProdutosDTO.desc,
                            price = todosProdutosDTO.price,
                            rrp = todosProdutosDTO.rrp,
                            quantity = todosProdutosDTO.quantity,
                            img = todosProdutosDTO.img,
                            type = todosProdutosDTO.type
                        };

                        await db.InsertAsync("products", "id", true, novoProduto);
                    }
                    else
                    {
                        // O produto já existe na base de dados, então vamos atualizá-lo
                        produtoExistente.Id = todosProdutosDTO.Id;
                        produtoExistente.name = todosProdutosDTO.name;
                        produtoExistente.desc = todosProdutosDTO.desc;
                        produtoExistente.price = todosProdutosDTO.price;
                        produtoExistente.rrp = todosProdutosDTO.rrp;
                        produtoExistente.quantity = todosProdutosDTO.quantity;
                        produtoExistente.img = todosProdutosDTO.img;
                        produtoExistente.type = todosProdutosDTO.type;

                        await db.UpdateAsync("products", "id", produtoExistente);
                    }
                }
            }

            return Ok();
        }



        [HttpPost("EliminarProdutos")]
        public async Task<ActionResult> DeleteTodosProdutos([FromBody] List<long> idList)
        {
            using (var db = new Database(connectionString, "MySql.Data.MySqlClient")) 
            {
                foreach (var id in idList)
                {
                    await db.DeleteAsync("products", "id", null, id);
                }
            }

            return Ok();
        }




        [HttpPost("Checkout")]
        public async Task<ActionResult<List<TodosProdutosDTO>>> Checkout([FromBody] List<TodosProdutosDTO> selectedItems)
        {
            try
            {
                using (var db = new Database(connectionString, "MySql.Data.MySqlClient")) 
                {
                    var quantityToRemove = new Dictionary<string, int>();

                    foreach (var item in selectedItems)
                    {
                        if (quantityToRemove.ContainsKey(item.name))
                        {
                            quantityToRemove[item.name] += 1;
                        }
                        else
                        {
                            quantityToRemove[item.name] = 1;
                        }
                    }

                    var updatedItems = new List<TodosProdutos>();

                    foreach (var itemName in quantityToRemove.Keys)
                    {
                        var quantity = quantityToRemove[itemName];

                        var todosProdutos = await db.SingleOrDefaultAsync<TodosProdutos>("SELECT * FROM products WHERE name = @0", itemName);

                        if (todosProdutos != null)
                        {
                            todosProdutos.quantity -= quantity;

                            updatedItems.Add(todosProdutos);

                            await db.UpdateAsync("products", "id", todosProdutos);
                        }
                    }

                    var allProducts = await db.FetchAsync<TodosProdutos>("SELECT * FROM products");

                    var responseItems = allProducts.Select(p => new TodosProdutosDTO
                    {
                        Id = p.Id,
                        name = p.name,
                        desc = p.desc,
                        price = p.price,
                        rrp = p.rrp,
                        quantity = p.quantity,
                        img = p.img,
                        type = p.type
                    }).ToList();

                    return Ok(responseItems);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Erro ao atualizar os produtos no banco de dados: " + ex.Message);
                return StatusCode(500, "Erro ao atualizar os produtos no banco de dados.");
            }
        }























        private bool TodoProdutosExists(long id)
        {
            return _context.TodoProdutos.Any(e => e.Id == id);
        }

        private static TodosProdutosDTO ProdutosToDTO(TodosProdutos todoProdutos) =>
           new TodosProdutosDTO
           {

               Id = todoProdutos.Id,
               name = todoProdutos.name,
               desc = todoProdutos.desc,
               price = todoProdutos.price,
               rrp = todoProdutos.rrp,
               quantity = todoProdutos.quantity,
               img = todoProdutos.img,
               type = todoProdutos.type
           };



    }
}
