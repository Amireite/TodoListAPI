using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TodoListAPI.Entities
{
    public class TodoItem
    {
        public string TodoId { get; set; }
        [JsonIgnore]
        public string UserId { get; set; }
        public string Content { get; set; }
        public DateTime? Deadline { get; set; }
        public bool IsDone { get; set; }
        public int Order { get; set; }
        [JsonIgnore]
        public User User { get; set; }
    }
}
