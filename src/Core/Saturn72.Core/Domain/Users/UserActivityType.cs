﻿using System;

namespace Saturn72.Core.Domain.Users
{
    public class UserActivityType
    {
        public static readonly UserActivityType UserRegistered = new UserActivityType("register", "85720C3A-F47B-475D-80B7-7738DCDBA12F", "UserActivityType.Register");

        public static readonly UserActivityType UserLoggedIn = new UserActivityType("login", "F30ED5D9-A03F-4928-9336-1B616CCB54D4", "UserActivityType.UserLoggedIn");


        protected UserActivityType(string name, string code, string systemName)
        {
            Name = name;
            Code = code;
            SystemName = systemName;
        }

        public string Name { get; }
        public string Code { get; }
        public string SystemName { get; }
    }
}