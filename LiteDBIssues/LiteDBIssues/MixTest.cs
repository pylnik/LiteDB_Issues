using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LiteDB;

namespace LiteDBIssues
{
    public class MixTest
    {
        private const string ThresholdTypeCollectionName = "thd_types";

        private BsonMapper _bsonMapperMain;

        public BsonMapper InitMapper()
        {
            _bsonMapperMain = new CustomMapper();
            _bsonMapperMain
                .Entity<TypeContainer>()
                .Id(ip => ip.Id)
                .DbRef(f => f.AllowedThresholds, ThresholdTypeCollectionName);
            _bsonMapperMain
                .Entity<IThresholdType>();
            _bsonMapperMain
                .Entity<MixBO>()
                .Id(m => m.Id)
                .DbRef(m => m.MeasurementFunctionType);
            return _bsonMapperMain;
        }

        public void Do()
        {
            var thdType1 = new ThresholdTypeEntity { ThresholdTypeGUID = "type 1" };
            var thdType2 = new ThresholdTypeEntity { ThresholdTypeGUID = "type 2" };


            var functionType = new TypeContainer
            {
                FunctionTypeID = "fId",
                AllowedThresholds = new List<IThresholdType>
                {
                    thdType1,
                    thdType2
                }
            };

            var mix = new MixBO
            {
                MeasurementFunctionType = functionType
            };

            var mapper = InitMapper();

            using (var ms = new MemoryStream())
            {
                var liteRepo = new LiteRepository(ms, mapper);
                liteRepo.Insert(thdType1, ThresholdTypeCollectionName);
                liteRepo.Insert(thdType2, ThresholdTypeCollectionName);
                liteRepo.Insert(functionType);
                liteRepo.Insert(mix);

                var liteRepo2 = new LiteRepository(ms, mapper);
                var mixList = liteRepo2.Database.GetCollection<MixBO>()
                    .IncludeAll()// IncludeAll() deserialize all the objects marked as DbRef
                    .FindAll()
                    .ToList();

                Debug.WriteLine($"mixList.Count = {mixList.Count}");
            }
        }
    }
}
