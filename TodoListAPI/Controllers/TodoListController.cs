using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoListAPI.Contexts;
using TodoListAPI.Entities;

namespace TodoListAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TodoListController : ControllerBase
    {
        private TodoContext _tdContext;
        public TodoListController(TodoContext context)
        {
            _tdContext = context;
        }
        [HttpGet]
        public IActionResult GetList()
        {
            try
            {
                var todoList = _tdContext.TodoItems.ToList().FindAll(td => td.UserId == GetUserId());
                if (todoList.Count == 0)
                {
                    return Ok(new
                    {
                        data = new List<TodoItem>()
                    });
                }

                return Ok(new
                {
                    data = todoList
                });
            } catch(Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddNewTodoItem([FromBody] NewItemBody body)
        {
            try
            {
                var user = await _tdContext.Users.Include(u => u.TodoList).FirstOrDefaultAsync(u => u.Id == GetUserId());
                if (user == null)
                {
                    return NotFound();
                }

                if (user.TodoList == null)
                {
                    user.TodoList = new List<TodoItem>();
                }

                if(user.TodoList.Count >= 20)
                {
                    return BadRequest("Too many todo items!");
                }

                user.TodoList.Add(new TodoItem
                {
                    Content = body.Content,
                    User = user,
                    Deadline = body.Deadline,
                    Order = body.Order
                });

                await _tdContext.SaveChangesAsync();
                return Ok();
            } catch(Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateTodoItem([FromBody] UpdateItemBody body)
        {
            try
            {
                var user = await _tdContext.Users.Include(u => u.TodoList).FirstOrDefaultAsync(u => u.Id == GetUserId());
                if(user == null)
                {
                    return NotFound();
                }

                var item = user.TodoList.FirstOrDefault(i => i.TodoId == body.Id);
                if(item == null)
                {
                    return NotFound();
                }

                item.Content = body.Content ?? item.Content;
                item.Order = body.Order ?? item.Order;
                item.Deadline = body.Deadline ?? item.Deadline;
                item.IsDone = body.IsDone ?? item.IsDone;
                await _tdContext.SaveChangesAsync();

                return Ok(new 
                {
                    item.TodoId,
                    item.Content,
                    item.IsDone,
                    item.Order,
                    item.Deadline
                });
            } catch(Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(string id)
        {
            try
            {
                var user = await _tdContext.Users.Include(u => u.TodoList).FirstOrDefaultAsync(u => u.Id == GetUserId());
                if (user == null)
                {
                    return NotFound();
                }

                var item = user.TodoList.FirstOrDefault(i => i.TodoId == id);
                if(item == null)
                {
                    return NotFound();
                }

                user.TodoList.Remove(item);
                _tdContext.TodoItems.Remove(item);
                await _tdContext.SaveChangesAsync();

                return Ok();
            } catch(Exception err)
            {
                throw new Exception(err.Message);
            }
        }
        private string GetUserId()
        {
            return User.Claims.ToList().Find(c => c.Type == "userid").Value;
        }
        public class NewItemBody
        {
            public string Content { get; set; }
            public DateTime? Deadline { get; set; }
            public int Order { get; set; }
        }
        public class UpdateItemBody
        {
            public string Id { get; set; }
            public string Content { get; set; }
            public int? Order { get; set; }
            public DateTime? Deadline { get; set; }
            public bool? IsDone { get; set; }
        }
    }
}
