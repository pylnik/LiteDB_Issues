using System;
using System.Collections.Generic;
using System.Reflection;
using LiteDB;

namespace LiteDBIssues
{
    public class CustomMapper : BsonMapper
    {
        protected override IEnumerable<MemberInfo> GetTypeMembers(Type type)
        {
            var list = new List<MemberInfo>(base.GetTypeMembers(type));

            if (type.IsInterface)
            {
                foreach (var @interface in type.GetInterfaces())
                {
                    list.AddRange(this.GetTypeMembers(@interface));
                }
            }

            return list;
        }
    }
}