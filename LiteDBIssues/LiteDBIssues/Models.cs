using System.Collections.Generic;

namespace LiteDBIssues
{
    #region Base models
    public interface IEntity
    {
        int Id { get; set; }
    }

    public class Entity : IEntity
    {
        public int Id { get; set; }
    }
    #endregion

    #region Threshold type
    public interface IThresholdType : IEntity
    {
        string ThresholdTypeGUID { get; set; }
    }

    public class ThresholdTypeEntity : Entity, IThresholdType
    {
        public ThresholdTypeEntity() { }
        public string ThresholdTypeGUID { get; set; }
    }
    #endregion

    public class TypeContainer : Entity
    {
        public string FunctionTypeID { get; set; }

        public List<IThresholdType> AllowedThresholds { get; set; } = new List<IThresholdType>();
    }

    public class MixBO : Entity
    {
        public TypeContainer MeasurementFunctionType { get; set; }
    }
}
