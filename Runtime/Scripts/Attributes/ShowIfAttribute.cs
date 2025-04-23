using UnityEngine;

namespace YaGamesSDK
{
    public class ShowIfAttribute : PropertyAttribute
    {
        public string ConditionName { get; }

        public ShowIfAttribute(string conditionName)
        {
            ConditionName = conditionName;
        }
    }
}
