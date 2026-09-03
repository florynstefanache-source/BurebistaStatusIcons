using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using BurebistaStatusIcons;
using MelonLoader;
using Microsoft.CodeAnalysis;
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints)]
[assembly: MelonInfo(typeof(BurebistaStatusIcons.Main), "Burebista Status Icons", "1.3.0", "Burebista", null)]
[assembly: MelonGame("Hinterland", "TheLongDark")]
[assembly: TargetFramework(".NETCoreApp,Version=v6.0", FrameworkDisplayName = ".NET 6.0")]
[assembly: AssemblyVersion("0.0.0.0")]
namespace Microsoft.CodeAnalysis
{
	[CompilerGenerated]
	[Microsoft.CodeAnalysis.Embedded]
	internal sealed class EmbeddedAttribute : Attribute
	{
	}
}
namespace BurebistaStatusIcons
{
	public class Main : MelonMod
	{
		private class StatusBadge
		{
			public string Icon;

			public string Label;

			public Color Color;

			public StatusBadge(string icon, string label, Color color)
			{
				//IL_0015: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Unknown result type (might be due to invalid IL or missing references)
				Icon = icon;
				Label = label;
				Color = color;
			}
		}

		private static GUIStyle box;

		private static GUIStyle statusIcon;

		private static GUIStyle statusLabel;

		private static Texture2D px;

		private static bool guiReady;

		private static float nextStatusUpdate;

		private static readonly List<StatusBadge> activeBadges = new List<StatusBadge>();

		private static readonly Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

		private static readonly Dictionary<string, MemberInfo[]> memberCache = new Dictionary<string, MemberInfo[]>();

		private static readonly HashSet<string> debugDumped = new HashSet<string>();

		public override void OnInitializeMelon()
		{
			((MelonBase)this).LoggerInstance.Msg("Burebista Status Icons v1.3 UN ICONO POR ESTADO loaded.");
		}

		public override void OnUpdate()
		{
			if (IsPlayable())
			{
				float unscaledTime = Time.unscaledTime;
				if (unscaledTime >= nextStatusUpdate)
				{
					nextStatusUpdate = unscaledTime + 3f;
					UpdateStatusBadges();
				}
			}
		}

		public override void OnGUI()
		{
			if (IsPlayable())
			{
				EnsureGUI();
				float x = (float)Screen.width - 104f;
				float num = 150f;
				for (int i = 0; i < activeBadges.Count; i++)
				{
					DrawBadge(x, num + (float)i * 86f, activeBadges[i]);
				}
			}
		}

		private static void DrawBadge(float x, float y, StatusBadge b)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			Color color = GUI.color;
			GUI.color = new Color(0.02f, 0.025f, 0.03f, 0.82f);
			GUI.Box(new Rect(x, y, 86f, 78f), "", box);
			GUI.color = color;
			statusIcon.normal.textColor = b.Color;
			statusLabel.normal.textColor = b.Color;
			GUI.Label(new Rect(x + 5f, y + 2f, 76f, 48f), b.Icon, statusIcon);
			GUI.Label(new Rect(x + 3f, y + 51f, 80f, 20f), b.Label, statusLabel);
		}

		private static float? DirectNeed(string getter, params string[] memberNames)
		{
			try
			{
				object obj = StaticCallAny("GameManager", getter);
				if (obj == null)
				{
					return null;
				}
				return Num(ExactMember(obj, memberNames));
			}
			catch
			{
				return null;
			}
		}

		private static bool DirectBool(string getter, params string[] memberNames)
		{
			try
			{
				object obj = StaticCallAny("GameManager", getter);
				if (obj == null)
				{
					return false;
				}
				object obj2 = ExactMember(obj, memberNames);
				return obj2 is bool && (bool)obj2;
			}
			catch
			{
				return false;
			}
		}

		private static void UpdateStatusBadges()
		{
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_0292: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0333: Unknown result type (might be due to invalid IL or missing references)
			//IL_0309: Unknown result type (might be due to invalid IL or missing references)
			//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0475: Unknown result type (might be due to invalid IL or missing references)
			//IL_044b: Unknown result type (might be due to invalid IL or missing references)
			//IL_056f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0545: Unknown result type (might be due to invalid IL or missing references)
			activeBadges.Clear();
			object o = StaticCallAny("GameManager", "GetHungerComponent");
			float? num = Num(ExactMember(o, "m_CurrentReserveCalories", "m_CurrentReserveCaloriesProxy", "m_CaloriesRemaining", "GetCurrentReserveCalories"));
			if (!num.HasValue)
			{
				num = FindNumber(o, new string[2] { "reserve", "calor" }, new string[2] { "current", "calor" });
			}
			float? num2 = Num(ExactMember(o, "m_MaxReserveCalories", "m_MaxReserveCaloriesProxy", "m_MaxCalories", "GetMaxReserveCalories"));
			if (!num2.HasValue)
			{
				num2 = FindNumber(o, new string[3] { "max", "reserve", "calor" }, new string[2] { "max", "calor" });
			}
			bool flag = false;
			if (num.HasValue)
			{
				flag = ((num2.HasValue && num2.Value > 100f) ? (num.Value / num2.Value <= 0.25f) : (num.Value <= 600f));
			}
			bool flag2 = false;
			bool? flag3 = BoolValue(ExactMember(StaticCallAny("GameManager", "GetWellFedComponent", "GetWellFed"), "m_Active", "IsActive", "IsWellFed", "m_IsActive"));
			if (!flag3.HasValue)
			{
				flag3 = BoolValue(ExactMember(o, "IsWellFed", "m_WellFed", "m_IsWellFed"));
			}
			flag2 = flag3.GetValueOrDefault();
			if (flag)
			{
				activeBadges.Add(new StatusBadge("◆", "HUNGRY", new Color(1f, 0.18f, 0.14f, 1f)));
			}
			else if (flag2)
			{
				activeBadges.Add(new StatusBadge("◆", "WELL FED", new Color(0.2f, 0.95f, 0.32f, 1f)));
			}
			else
			{
				activeBadges.Add(new StatusBadge("◆", "FED", new Color(0.2f, 0.82f, 0.32f, 1f)));
			}
			bool flag4 = IsBadHigh(DirectNeed("GetThirstComponent", "m_CurrentThirst", "m_CurrentThirstProxy", "m_Thirst", "GetThirst"), 0.7f, 70f);
			activeBadges.Add(flag4 ? new StatusBadge("●", "THIRSTY", new Color(1f, 0.16f, 0.14f, 1f)) : new StatusBadge("●", "HYDRATED", new Color(0.18f, 0.66f, 1f, 1f)));
			bool flag5 = IsBadHigh(DirectNeed("GetFatigueComponent", "m_CurrentFatigue", "m_CurrentFatigueProxy", "m_Fatigue", "GetFatigue"), 0.75f, 75f);
			activeBadges.Add(flag5 ? new StatusBadge("Z", "EXHAUSTED", new Color(1f, 0.16f, 0.14f, 1f)) : new StatusBadge("Z", "RESTED", new Color(0.68f, 0.46f, 1f, 1f)));
			bool flag6 = IsBadHigh(DirectNeed("GetFreezingComponent", "m_CurrentFreezing", "m_CurrentFreezingProxy", "m_Freezing", "GetFreezing"), 0.55f, 55f);
			activeBadges.Add(flag6 ? new StatusBadge("✣", "FREEZING", new Color(1f, 0.16f, 0.14f, 1f)) : new StatusBadge("✣", "WARM", new Color(0.25f, 0.86f, 1f, 1f)));
			float? num3 = DirectNeed("GetConditionComponent", "m_CurrentHP", "m_CurrentHPProxy", "m_Condition", "GetCondition");
			float? num4 = DirectNeed("GetConditionComponent", "m_MaxHP", "m_MaxCondition", "GetMaxCondition");
			bool flag7 = num3.HasValue && num4.HasValue && num4.Value > 0f && num3.Value / num4.Value <= 0.7f;
			activeBadges.Add(flag7 ? new StatusBadge("!", "INJURED", new Color(1f, 0.16f, 0.14f, 1f)) : new StatusBadge("+", "HEALTHY", new Color(0.24f, 0.95f, 0.32f, 1f)));
		}

		private static bool IsBadHigh(float? value, float normalizedThreshold, float percentThreshold)
		{
			if (!value.HasValue)
			{
				return false;
			}
			float value2 = value.Value;
			if (value2 <= 1.5f)
			{
				return value2 >= normalizedThreshold;
			}
			return value2 >= percentThreshold;
		}

		private static bool? BoolValue(object v)
		{
			if (v is bool)
			{
				return (bool)v;
			}
			return null;
		}

		private static void EnsureGUI()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Expected O, but got Unknown
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Expected O, but got Unknown
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Expected O, but got Unknown
			if (!guiReady)
			{
				px = new Texture2D(1, 1);
				px.SetPixel(0, 0, new Color(0.035f, 0.04f, 0.045f, 0.76f));
				px.Apply();
				box = new GUIStyle(GUI.skin.box);
				box.normal.background = px;
				statusIcon = new GUIStyle(GUI.skin.label);
				statusIcon.fontSize = 38;
				statusIcon.fontStyle = (FontStyle)1;
				statusIcon.alignment = (TextAnchor)4;
				statusLabel = new GUIStyle(GUI.skin.label);
				statusLabel.fontSize = 11;
				statusLabel.fontStyle = (FontStyle)1;
				statusLabel.alignment = (TextAnchor)4;
				guiReady = true;
			}
		}

		private static bool IsPlayable()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Scene activeScene = SceneManager.GetActiveScene();
				string text = (activeScene.name ?? "").ToLowerInvariant();
				if (text.Length == 0 || text.Contains("boot") || text.Contains("menu") || text.Contains("loading") || text.Contains("empty"))
				{
					return false;
				}
				return StaticCallAny("GameManager", "GetPlayerTransform") != null;
			}
			catch
			{
				return false;
			}
		}

		private static bool IsOutdoorScene()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Scene activeScene = SceneManager.GetActiveScene();
			string text = (activeScene.name ?? "").ToLowerInvariant();
			if (text.Contains("region"))
			{
				return true;
			}
			if (text.Contains("airfield"))
			{
				return true;
			}
			if (text.Contains("blackrock"))
			{
				return true;
			}
			if (text.Contains("transferpass"))
			{
				return true;
			}
			return false;
		}

		private static Type T(string name)
		{
			if (typeCache.TryGetValue(name, out var value))
			{
				return value;
			}
			Type type = null;
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				try
				{
					type = assembly.GetType(name, throwOnError: false);
					if (type != null)
					{
						break;
					}
				}
				catch
				{
				}
			}
			if (type == null)
			{
				assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (Assembly assembly2 in assemblies)
				{
					Type[] types;
					try
					{
						types = assembly2.GetTypes();
					}
					catch
					{
						continue;
					}
					Type[] array = types;
					foreach (Type type2 in array)
					{
						if (string.Equals(type2.Name, name, StringComparison.OrdinalIgnoreCase))
						{
							type = type2;
							break;
						}
					}
					if (type != null)
					{
						break;
					}
				}
			}
			typeCache[name] = type;
			return type;
		}

		private static object StaticCallAny(string type, params string[] methods)
		{
			Type type2 = T(type);
			if (type2 == null)
			{
				return null;
			}
			foreach (string b in methods)
			{
				try
				{
					MethodInfo[] methods2 = type2.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
					foreach (MethodInfo methodInfo in methods2)
					{
						if (string.Equals(methodInfo.Name, b, StringComparison.OrdinalIgnoreCase) && methodInfo.GetParameters().Length == 0)
						{
							return methodInfo.Invoke(null, null);
						}
					}
				}
				catch
				{
				}
			}
			return null;
		}

		private static MemberInfo[] Members(object o)
		{
			if (o == null)
			{
				return new MemberInfo[0];
			}
			string assemblyQualifiedName = o.GetType().AssemblyQualifiedName;
			if (memberCache.TryGetValue(assemblyQualifiedName, out var value))
			{
				return value;
			}
			List<MemberInfo> list = new List<MemberInfo>();
			Type type = o.GetType();
			try
			{
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (PropertyInfo propertyInfo in properties)
				{
					if (propertyInfo.GetIndexParameters().Length == 0)
					{
						list.Add(propertyInfo);
					}
				}
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo item in fields)
				{
					list.Add(item);
				}
				MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo in methods)
				{
					if (methodInfo.GetParameters().Length == 0 && !(methodInfo.ReturnType == typeof(void)) && (methodInfo.Name.StartsWith("Get") || methodInfo.Name.StartsWith("Is")))
					{
						list.Add(methodInfo);
					}
				}
			}
			catch
			{
			}
			value = list.ToArray();
			memberCache[assemblyQualifiedName] = value;
			return value;
		}

		private static object GetValue(object o, MemberInfo m)
		{
			try
			{
				PropertyInfo propertyInfo = m as PropertyInfo;
				if (propertyInfo != null)
				{
					return propertyInfo.GetValue(o);
				}
				FieldInfo fieldInfo = m as FieldInfo;
				if (fieldInfo != null)
				{
					return fieldInfo.GetValue(o);
				}
				MethodInfo methodInfo = m as MethodInfo;
				if (methodInfo != null)
				{
					return methodInfo.Invoke(o, null);
				}
			}
			catch
			{
			}
			return null;
		}

		private static object ExactMember(object o, params string[] names)
		{
			if (o == null)
			{
				return null;
			}
			Type type = o.GetType();
			foreach (string name in names)
			{
				try
				{
					PropertyInfo property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (property != null && property.GetIndexParameters().Length == 0)
					{
						return property.GetValue(o);
					}
					FieldInfo field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (field != null)
					{
						return field.GetValue(o);
					}
					MethodInfo method = type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
					if (method != null)
					{
						return method.Invoke(o, null);
					}
				}
				catch
				{
				}
			}
			return null;
		}

		private static float? Num(object o)
		{
			if (o == null)
			{
				return null;
			}
			try
			{
				return Convert.ToSingle(o);
			}
			catch
			{
				return null;
			}
		}

		private static float? FindNumber(object o, params string[][] groups)
		{
			if (o == null)
			{
				return null;
			}
			MemberInfo[] array = Members(o);
			foreach (MemberInfo memberInfo in array)
			{
				string text = memberInfo.Name.ToLowerInvariant();
				foreach (string[] obj in groups)
				{
					bool flag = true;
					string[] array2 = obj;
					foreach (string text2 in array2)
					{
						if (!text.Contains(text2.ToLowerInvariant()))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						float? result = Num(GetValue(o, memberInfo));
						if (result.HasValue && !float.IsNaN(result.Value) && !float.IsInfinity(result.Value))
						{
							return result;
						}
					}
				}
			}
			return null;
		}

		private static bool? FindBool(object o, params string[][] groups)
		{
			if (o == null)
			{
				return null;
			}
			MemberInfo[] array = Members(o);
			foreach (MemberInfo memberInfo in array)
			{
				string text = memberInfo.Name.ToLowerInvariant();
				foreach (string[] obj in groups)
				{
					bool flag = true;
					string[] array2 = obj;
					foreach (string text2 in array2)
					{
						if (!text.Contains(text2.ToLowerInvariant()))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						object value = GetValue(o, memberInfo);
						if (value is bool)
						{
							return (bool)value;
						}
					}
				}
			}
			return null;
		}

		private static string Hm(float hours)
		{
			if (hours < 0f)
			{
				hours = 0f;
			}
			int num = (int)hours;
			int num2 = Mathf.Clamp((int)Math.Round((hours - (float)num) * 60f), 0, 59);
			if (num <= 0)
			{
				return num2 + "m";
			}
			return num + "h " + num2.ToString("00") + "m";
		}

		private static string ReadDaylight()
		{
			try
			{
				object obj = StaticCallAny("GameManager", "GetTimeOfDayComponent");
				if (obj == null)
				{
					return "N/A";
				}
				object obj2 = ExactMember(obj, "GetHoursDaylightString");
				if (obj2 is string && !string.IsNullOrEmpty((string)obj2))
				{
					return (string)obj2;
				}
			}
			catch
			{
			}
			return "N/A";
		}

		private static string ReadThermal()
		{
			try
			{
				object obj = StaticCallAny("GameManager", "GetWeatherComponent");
				if (obj == null)
				{
					return "N/A";
				}
				float? num = Num(ExactMember(obj, "GetCurrentTemperature", "m_CurrentTemperature"));
				if (num.HasValue && num.Value > -150f && num.Value < 100f)
				{
					return num.Value.ToString("+0.0;-0.0;0.0") + "°C";
				}
			}
			catch
			{
			}
			return "N/A";
		}

		private static string ReadWind()
		{
			try
			{
				object obj = StaticCallAny("GameManager", "GetWeatherComponent");
				if (obj == null)
				{
					return "N/A";
				}
				object obj2 = ExactMember(obj, "IsIndoorEnvironment", "m_IsIndoors");
				if (obj2 is bool && (bool)obj2)
				{
					return "INDOORS";
				}
				float? num = Num(ExactMember(obj, "m_CurrentWindChill"));
				if (num.HasValue)
				{
					if (Math.Abs(num.Value) < 0.05f)
					{
						return "CALM";
					}
					return num.Value.ToString("0.0");
				}
			}
			catch
			{
			}
			return "N/A";
		}

		private static void DumpOnce(object o, string tag)
		{
			if (o == null)
			{
				return;
			}
			string item = tag + ":" + o.GetType().FullName;
			if (debugDumped.Contains(item))
			{
				return;
			}
			debugDumped.Add(item);
			try
			{
				List<string> list = new List<string>();
				MemberInfo[] array = Members(o);
				foreach (MemberInfo memberInfo in array)
				{
					list.Add(memberInfo.Name);
					if (list.Count >= 120)
					{
						break;
					}
				}
				MelonLogger.Msg("[BurebistaHUD DEBUG " + tag + "] " + o.GetType().FullName + ": " + string.Join(", ", list.ToArray()));
			}
			catch
			{
			}
		}
	}
}
