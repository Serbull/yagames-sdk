using UnityEngine;

namespace YaGamesSDK.Core
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
