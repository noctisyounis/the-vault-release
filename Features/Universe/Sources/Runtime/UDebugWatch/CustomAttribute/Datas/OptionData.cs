using System;

namespace Universe
{
	[Serializable]
	public struct OptionData
	{
		public string m_name;
		public object m_value;

		public OptionData(string name, object value)
		{
			m_name = name;
			m_value = value;
		}

		public override string ToString() =>
			m_name;
	}
}