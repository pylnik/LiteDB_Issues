using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LiteDB;

namespace LiteDBIssues
{
    public interface INode : IEntity
    {
        string NodeName { get; set; }
    }
    public class Leaf : Entity, INode
    {
        public string NodeName { get; set; }
    }
    public class Branch : Entity, INode
    {
        public string NodeName { get; set; }
        public INode NodeLeft { get; set; }
        public INode NodeRight { get; set; }
    }

    public class TreeTest
    {
        private const string NodesCollectionName = "Nodes";

        private BsonMapper _bsonMapperMain;

        public BsonMapper InitMapper()
        {
            _bsonMapperMain = new CustomMapper();
            _bsonMapperMain
                .Entity<INode>()
                .Id(n => n.Id);
            _bsonMapperMain
                .Entity<Leaf>()
                .Id(leaf => leaf.Id);
            _bsonMapperMain
                .Entity<Branch>()
                .Id(branch => branch.Id)
                .DbRef(branch => branch.NodeLeft)
                .DbRef(branch => branch.NodeRight)
                ;
            return _bsonMapperMain;
        }

        public void Do()
        {
            Leaf leaf1 = new Leaf { NodeName = "Leaf 1" };
            Leaf leaf2 = new Leaf { NodeName = "Leaf 2" };
            Leaf leaf3 = new Leaf { NodeName = "Leaf 3" };

            Branch branch1 = new Branch { NodeName = "Branch 1" };
            Branch branch2 = new Branch { NodeName = "Branch 2" };

            branch1.NodeLeft = leaf1;
            branch1.NodeRight = branch2;

            branch2.NodeLeft = leaf2;
            branch2.NodeRight = leaf3;

            var mapper = InitMapper();

            using (var ms = new MemoryStream())
            {
                var liteRepo = new LiteRepository(ms, mapper);
                liteRepo.Insert(leaf1, NodesCollectionName);
                liteRepo.Insert(leaf2, NodesCollectionName);
                liteRepo.Insert(leaf3, NodesCollectionName);
                liteRepo.Insert(branch1, NodesCollectionName);
                liteRepo.Insert(branch2, NodesCollectionName);

                var liteRepo2 = new LiteRepository(ms, mapper);
                var nodes = liteRepo2.Database.GetCollection<INode>(NodesCollectionName)
                    .IncludeAll()// IncludeAll() deserialize all the objects marked as DbRef
                    .FindAll()
                    .ToList();

                Debug.WriteLine($"first node = {nodes.FirstOrDefault()?.NodeName}");
            }
        }
    }
}
