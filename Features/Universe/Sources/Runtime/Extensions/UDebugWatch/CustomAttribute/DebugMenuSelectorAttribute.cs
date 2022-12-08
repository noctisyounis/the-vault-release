using System;

namespace Universe.DebugWatch.Runtime
{
	[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
	public class DebugMenuSelectorAttribute : DebugMenuAttribute
	{
		#region Public Properties

		public OptionData[] Options => _options;

		#endregion


		#region Constructor

		public DebugMenuSelectorAttribute(string path, string[] optionNames, object[] options, string tooltip = "", int sortingOrder = 0) : base(path, tooltip, sortingOrder)
		{
			var optionLength = options.Length;
			var nameLength = optionNames.Length;
			
			_options = new OptionData[optionLength];
			for (var i = 0; i < optionLength; i++)
			{
				var option = options[i];
				var displayName = i >= nameLength ? option.ToString() : optionNames[i];
				var data = new OptionData()
				{
					m_name = displayName,
					m_value = option
				};

				_options[i] = data;
			}
		}

		#endregion
		
		
		#region Main

		public override OptionData[] GetOptions()
			=> Options;

		#endregion


		#region Private Members

		protected OptionData[] _options;

		#endregion
	}
}