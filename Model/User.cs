﻿using System.Data;
using System.Text.Json.Serialization;

namespace UsersApi.Model
{
        public class User
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public int Age { get; set; }
            public string? Email{ get; set; }   
            public List<UserRole>? userRoles { get; set; }
        }
}
