﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoListAPI.Entities
{
    public class SignUpBody
    {
        public string email { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
}
