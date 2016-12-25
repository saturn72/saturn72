using System;

namespace Saturn72.Core.Domain.Users
{
    public class UserActivityType
    {
        public static readonly UserActivityType Register = new UserActivityType("register",
            new Guid("85720C3A-F47B-475D-80B7-7738DCDBA12F"));

        private UserActivityType(string name, Guid code)
        {
            Name = name;
            Code = code;
        }

        public string Name { get; }
        public Guid Code { get; }
    }
}