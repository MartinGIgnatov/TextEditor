using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextEditorMVC.CustomClasses
{
    public class User
    {
        public User(Guid id, string username, string password, string email, DateTime creationDate, DateTime lastTimeOnline, string name)
        {
            Id = id;
            Username = username;
            Password = password;
            Email = email;
            CreationDate = creationDate;
            LastTimeOnline = lastTimeOnline;
            Name = name;
        }

        public Guid Id { get; }

        public string Username { get; }

        public string Password { get; }

        public string Email { get; }

        public DateTime CreationDate { get; }

        public DateTime LastTimeOnline { get; }

        public string Name { get; }
    }
}
