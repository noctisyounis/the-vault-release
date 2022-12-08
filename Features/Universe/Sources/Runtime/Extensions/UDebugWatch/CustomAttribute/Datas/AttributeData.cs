using System;
using System.Reflection;

namespace Universe.DebugWatch.Runtime
{
	[Serializable]
	public struct AttributeData
	{
		public Type m_attributeType;
		public MethodInfo m_method;
		public OptionData[] m_options;
		public object m_lastResult;

		public string[] GetOptionNames()
		{
			var length = m_options.Length;
			var names = new string[length];

			for (var i = 0; i < length; i++)
			{
				var option = m_options[i];
				var name = option.m_name;

				names[i] = name;
			}

			return names;
		}
	}
}