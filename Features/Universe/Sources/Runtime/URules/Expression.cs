using System.Reflection;
using UnityEngine;

namespace Universe
{
    public class Expression : ScriptableObject
    {
        public ScriptableObject m_firstOperand;
        public PropertyInfo _firstOperandProperty;
        public BooleanOperator m_operator;
        public string m_secondOperand;
    }
    
    public enum BooleanOperator
    {
        Equals,
        NotEquals,
        Superior,
        Inferior,
        SuperiorOrEquals,
        InferiorOrEquals,
        Exist,
        Dummy
    }
}