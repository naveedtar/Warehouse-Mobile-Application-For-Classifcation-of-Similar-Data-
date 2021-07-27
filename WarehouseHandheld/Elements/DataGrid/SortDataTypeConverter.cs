﻿using System;

namespace Xamarin.Forms.DataGridLocal
{
	public class SortDataTypeConverter : TypeConverter
	{

		public override bool CanConvertFrom(Type sourceType)
		{
			return base.CanConvertFrom(sourceType);
		}

		public override object ConvertFromInvariantString(string value)
		{
			int index = 0;

			if (int.TryParse(value, out index))
				return (SortData)index;
			else
				return null;
		}
	}
}
