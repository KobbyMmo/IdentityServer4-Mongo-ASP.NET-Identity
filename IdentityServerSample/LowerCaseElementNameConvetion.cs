using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerSample
{
    public class LowerCaseElementNameConvetion : IMemberMapConvention
    {
        public string Name => nameof(LowerCaseElementNameConvetion);
        public void Apply(BsonMemberMap memberMap)
        {
            memberMap.SetElementName(memberMap.ElementName.ToLower());
        }
    }
}
