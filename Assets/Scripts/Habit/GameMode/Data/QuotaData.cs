using SimpleJSON;
using UnityEngine;

namespace Habillage
{
    public class QuotaData : ModeData
    {
        public override GameModeType Type => GameModeType.Quota;

        public Vector2 IdealValue;

        public QuotaData(TimeData _notify, Vector2 _value) : base(_notify)
        {
            IdealValue = _value;
        }

        public override string ToString()
        {
            return $"{IdealValue.x}, {IdealValue.y}";
        }

        public override JSONObject SerializeData()
        {
            var _json = base.SerializeData();
            _json.Add("ideal", IdealValue);

            return _json;
        }

        public override void DeserializeData(JSONObject _json)
        {
            base.DeserializeData(_json);

            IdealValue = _json["ideal"];
        }
    }
}