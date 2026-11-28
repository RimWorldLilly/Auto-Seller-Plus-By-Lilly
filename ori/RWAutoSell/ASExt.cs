using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000005 RID: 5
	[NullableContext(1)]
	[Nullable(0)]
	public static class ASExt
	{
		// Token: 0x06000018 RID: 24 RVA: 0x00002408 File Offset: 0x00000608
		private static bool FirstStartsWith(this IList<string> This, string startstring, out string secondstring)
		{
			int num = This.FirstIndexOf((string x) => x.StartsWith(startstring));
			if (num == -1)
			{
				secondstring = string.Empty;
				return false;
			}
			secondstring = This[num].Substring(startstring.Length);
			return true;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000245C File Offset: 0x0000065C
		public static string[] AllStartsWith(this IList<string> strings, string startstring)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < strings.Count<string>(); i++)
			{
				if (strings[i].StartsWith(startstring))
				{
					list.Add(strings[i].Substring(startstring.Length));
				}
			}
			return list.ToArray();
		}

		// Token: 0x0600001A RID: 26 RVA: 0x000024B0 File Offset: 0x000006B0
		public static List<T> AllStartsWith<[Nullable(2)] T>(this IList<string> strings, string startstring)
		{
			List<T> list = new List<T>();
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			if (converter != null)
			{
				for (int i = 0; i < strings.Count<string>(); i++)
				{
					if (strings[i].StartsWith(startstring))
					{
						string text = strings[i].Substring(startstring.Length);
						if (converter.IsValid(text))
						{
							list.Add((T)((object)converter.ConvertFromString(text)));
						}
					}
				}
			}
			return list;
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00002528 File Offset: 0x00000728
		public static List<T> AllStartsWithDefs<[Nullable(0)] T>(this IList<string> strings, string startstring) where T : Def
		{
			List<T> list = new List<T>();
			for (int i = 0; i < strings.Count<string>(); i++)
			{
				if (strings[i].StartsWith(startstring))
				{
					T named = DefDatabase<T>.GetNamed(strings[i].Substring(startstring.Length), false);
					if (named != null)
					{
						list.Add(named);
					}
				}
			}
			return list;
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002584 File Offset: 0x00000784
		[NullableContext(0)]
		public static void ChangeSubString<[IsUnmanaged] T>([Nullable(1)] this IList<string> strings, [Nullable(1)] string startstring, T value) where T : struct, ValueType
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			if (converter != null && strings != null)
			{
				int num = -1;
				if (strings.Count<string>() > 0)
				{
					num = strings.FirstIndexOf((string x) => x.StartsWith(startstring));
				}
				string text = startstring + converter.ConvertToString(value);
				if (num == -1)
				{
					strings.Add(text);
					return;
				}
				strings[num] = text;
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002600 File Offset: 0x00000800
		public static void ChangeSubString(this IList<string> strings, string startstring, string value)
		{
			if (strings != null)
			{
				int num = -1;
				if (strings.Count<string>() > 0)
				{
					num = strings.FirstIndexOf((string x) => x.StartsWith(startstring));
				}
				string text = startstring + value;
				if (num == -1)
				{
					strings.Add(text);
					return;
				}
				strings[num] = text;
			}
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000265C File Offset: 0x0000085C
		[NullableContext(0)]
		public static bool FirstSubString<[IsUnmanaged] T>([Nullable(1)] this IList<string> strings, [Nullable(1)] string startstring, out T ret, T init = default(T)) where T : struct, ValueType
		{
			string text;
			if (strings.FirstStartsWith(startstring, out text))
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
				if (converter != null && converter.IsValid(text))
				{
					ret = (T)((object)converter.ConvertFromString(text));
					return true;
				}
			}
			ret = init;
			return false;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000026AC File Offset: 0x000008AC
		public static bool FirstSubString(this IList<string> strings, string startstring, out string ret, string init = "")
		{
			string text;
			if (strings.FirstStartsWith(startstring, out text))
			{
				ret = text;
				return true;
			}
			ret = init;
			return false;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000026D0 File Offset: 0x000008D0
		public static bool FirstSubString(this IList<string> strings, string startstring, out bool retbool, bool init = false)
		{
			string text;
			if (strings.FirstStartsWith(startstring, out text))
			{
				string a = text.ToLower();
				if (!(a == "0") && !(a == "false"))
				{
					if (!(a == "1") && !(a == "true"))
					{
						retbool = init;
						return false;
					}
					retbool = true;
				}
				else
				{
					retbool = false;
				}
				return true;
			}
			retbool = init;
			return false;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x0000273C File Offset: 0x0000093C
		public static string[] AllSubString(this IList<string> strings, string startstring)
		{
			List<string> list = new List<string>();
			foreach (string text in strings)
			{
				if (text.StartsWith(startstring))
				{
					list.Add(text.Substring(startstring.Length));
				}
			}
			return list.ToArray();
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000027A4 File Offset: 0x000009A4
		public static string[] AllUniqueStartStrings(this IList<string> strings, char sepstring)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (string text in strings)
			{
				if (text.Contains(sepstring))
				{
					string[] array = text.Split(sepstring, StringSplitOptions.None);
					if (array.Length != 0)
					{
						hashSet.Add(array[0]);
					}
				}
			}
			return hashSet.ToArray<string>();
		}
	}
}
